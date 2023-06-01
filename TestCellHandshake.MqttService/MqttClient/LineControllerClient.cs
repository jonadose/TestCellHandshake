using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using TestCellHandshake.MqttService.MqttClient.ClientService;
using TestCellHandshake.MqttService.MqttClient.Service;

namespace TestCellHandshake.MqttService.MqttClient
{
    public class LineControllerClient : BackgroundService
    {
        private readonly ILogger<LineControllerClient> _logger;
        private readonly IMqttClientService _mqttClientService;
        private readonly ILogicHandlingService _logicHandlingService;

        public LineControllerClient(ILogger<LineControllerClient> logger,
            IMqttClientService mqttClientService,
            ILogicHandlingService logicHandlingService)
        {
            _logger = logger;
            _mqttClientService = mqttClientService;
            _logicHandlingService = logicHandlingService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting LineControllerClient");

            try
            {
                // Link the client service to the correct method to call when an event is triggered
                _mqttClientService.MethodThatTakesAction(MethodToHandleEvent);

                // Connect to mqtt broker
                await _mqttClientService.ConnectAsync();

                if (_mqttClientService.IsConnected())
                {
                    _logger.LogInformation("MqttClientService is connected");
                }

                // Subscribe to topic
                await _mqttClientService.SubscribeAsync("iotgateway/testcell");

                await Task.Delay(Timeout.Infinite, stoppingToken);
                await _mqttClientService.UnsubscribeAsync("iotgateway/testcell");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LineControllerClient ExecuteAsync.");
            }
        }


        public Task MethodToHandleEvent(MqttApplicationMessageReceivedEventArgs args)
        {
            try
            {
                _logger.LogInformation($"Message received: {Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment)}");
                _logicHandlingService.HandleApplicationMessageReceived(args);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling the MQTT application message.");
                throw;
            }
        }
    }
}
