
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using TestCellHandshake.MqttService.MqttService.Configuration;

namespace TestCellHandshake.MqttService.MqttClient.ClientService
{
    public class MqttClientService : IMqttClientService, IDisposable
    {
        private IManagedMqttClient? _mqttClient;
        private ManagedMqttClientOptions? _managedMqttClientOptions;
        private readonly ILogger<MqttClientService> _logger;
        private readonly IOptionsMonitor<MqttConfig> _mqttConfig;

        public MqttClientService(ILogger<MqttClientService> logger,
            IOptionsMonitor<MqttConfig> mqttConfig)
        {
            _logger = logger;
            _mqttConfig = mqttConfig;
            _mqttClient = Initialize(new MqttClientOptionsBuilder()
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


        public IManagedMqttClient Initialize(MqttClientOptions mqttClientOptions)
        {
            _managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();
            var factory = new MqttFactory();
            return _mqttClient = factory.CreateManagedMqttClient();
        }


        public bool IsConnected()
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            return _mqttClient.IsConnected;
        }


        public async Task SubscribeAsync(string topic)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            try
            {
                await _mqttClient.SubscribeAsync(topic);
                _logger.LogInformation($"Subscribed to topic: {topic}");
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


        public void MethodThatTakesAction(Func<MqttApplicationMessageReceivedEventArgs, Task> func)
        {
            ArgumentNullException.ThrowIfNull(_mqttClient);
            _mqttClient.ApplicationMessageReceivedAsync += func;

        }


        public void Dispose()
        {
            // TODO: Implement IDisposable properly 
            _mqttClient?.Dispose();
        }
    }
}
