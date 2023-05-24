using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCellHandshake.MqttService.MqttClient.ClientService;

namespace TestCellHandshake.MqttService.MqttClient
{
    public class LineControllerClient : BackgroundService
    {
        private readonly ILogger<LineControllerClient> _logger;

        private readonly IMqttClientService _mqttClientService;

        public LineControllerClient(ILogger<LineControllerClient> logger,

            IMqttClientService mqttClientService)
        {
            _logger = logger;
            _mqttClientService = mqttClientService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting LineControllerClient");

            try
            {
                // Connect to mqtt broker
                await _mqttClientService.ConnectAsync();

                if (_mqttClientService.IsConnected())
                {
                    _logger.LogInformation("MqttClientService is connected");
                }

                // Subscribe to topic
                await _mqttClientService.SubscribeAsync("iotgateway/linecontroller");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                    await _mqttClientService.UnsubscribeAsync("iotgateway/linecontroller");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LineControllerClient ExecuteAsync.");
            }
        }
    }
}
