using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System.Text;
using TestCellHandshake.MqttService.MqttService.Configuration;

namespace TestCellHandshake.MqttService.MqttService.Service
{
    public class MqttService : IMqttService
    {

        private IManagedMqttClient? _mqttClient;
        private ManagedMqttClientOptions? _managedMqttClientOptions;
        private readonly ILogger<MqttService> _logger;
        private readonly IOptionsMonitor<MqttConfig> _mqttConfig;

        public MqttService(ILogger<MqttService> logger, IOptionsMonitor<MqttConfig> mqttConfig)
        {
            _logger = logger;
            _mqttConfig = mqttConfig;
            Initialize(new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithCleanSession(true)
                .WithTcpServer(_mqttConfig.CurrentValue.BaseUrl)
                .Build());
        }


        public async Task ConnectAsync()
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            try
            {
                if (!_mqttClient.IsConnected) await _mqttClient.StartAsync(_managedMqttClientOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MQTT client failed to connect. Exception message: {ex.Message}");
            }
        }

        public void Initialize(MqttClientOptions mqttClientOptions)
        {
            _managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();
            var factory = new MqttFactory();
            _mqttClient = factory.CreateManagedMqttClient();
        }

        public bool IsConnected()
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            return _mqttClient.IsConnected;
        }

        public void MethodThatTakesAction(Func<MqttApplicationMessageReceivedEventArgs, Task> func)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            _mqttClient.ApplicationMessageReceivedAsync += func;
        }


        private async Task EnqueueAsync(MqttApplicationMessage message)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            try
            {
                await ConnectAsync();
                await _mqttClient.EnqueueAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MQTT client failed to publish. Exception message: {ex.Message}");
            }
        }


        private MqttApplicationMessage ConvertToMqttMessage(string topic, byte[] payload)
        {
            return new MqttApplicationMessage()
            {
                Topic = topic,
                Payload = payload,
                Retain = false,
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
            };
        }


        public async Task PublishAsync(string topic, string payload)
        {
            var message = ConvertToMqttMessage(topic, Encoding.UTF8.GetBytes(payload));
            await EnqueueAsync(message);
        }


        public async Task SubscribeAsync(string topic)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            try
            {
                await _mqttClient.SubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MQTT client failed to subscribe. Exception message: {ex.Message}");
            }
        }

        public async Task UnsubscribeAsync(string topic)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            try
            {
                await _mqttClient.UnsubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MQTT client failed to unsubscribe. Exception message: {ex.Message}");
            }
        }
    }
}
