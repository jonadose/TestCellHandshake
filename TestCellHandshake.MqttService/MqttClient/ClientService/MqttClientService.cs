
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using TestCellHandshake.MqttService.MqttService.Configuration;

namespace TestCellHandshake.MqttService.MqttClient.ClientService
{
    public class MqttClientService : IMqttClientService
    {
        private IManagedMqttClient? _mqttClient;
        private ManagedMqttClientOptions? _managedMqttClientOptions;
        private readonly ILogger<MqttClientService> _logger;
        private readonly IOptionsMonitor<MqttConfig> _mqttConfig;

        public MqttClientService(ILogger<MqttClientService> logger, IOptionsMonitor<MqttConfig> mqttConfig)
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
    }
}
