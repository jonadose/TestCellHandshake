using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System.Text;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;

namespace TestCellHandshake.MqttService.MqttClient
{
    public class LineControllerClient : BackgroundService
    {
        private IMqttClient? _mqttClient;
        private readonly ILogger<LineControllerClient> _logger;
        private readonly IPayloadParser _payloadParser;

        public LineControllerClient(ILogger<LineControllerClient> logger,
            IPayloadParser payloadParser)
        {
            _logger = logger;
            _payloadParser = payloadParser;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting LineControllerClient");

            var factory = new MqttFactory();
            using (_mqttClient = factory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("LineControllerClient")
                .WithTcpServer("127.0.0.1")
                .WithCleanSession(true)
                .Build();

                _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;

                await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var mqttSubscribeOptions = CreateMqttSubscribeOptions();

                var response = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
                _logger.LogInformation("MQTT client subscribed to topic.");
                _logger.LogInformation($"{response}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
            }
        }


        private MqttClientSubscribeOptions CreateMqttSubscribeOptions()
        {
            return new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(
                    filterBuilder =>
                    {
                        filterBuilder
                        .WithTopic("iotgateway/linecontroller");
                    })
                .Build();
        }


        private Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payload = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);
            _logger.LogInformation("Parsed payload.TagAddress: {address}, value : {value}", payload.TagAddress, payload.Value);
            return Task.CompletedTask;
        }
    }
}
