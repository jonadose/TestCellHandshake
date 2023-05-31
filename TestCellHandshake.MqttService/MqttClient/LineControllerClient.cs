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


        public void MethodToHandleEvent(MqttApplicationMessageReceivedEventArgs args)
        {
            _logger.LogInformation($"Message received: {Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment)}");
            _logicHandlingService.HandleApplicationMessageReceived(args);

            //_mqttClientService.MethodThatTakesAction();
        }

    }
}
