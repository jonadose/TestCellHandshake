

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TestCellHandshake.MqttService.Channels.TestCell;
using TestCellHandshake.MqttService.Commands.TestCell;
using TestCellHandshake.MqttService.MqttService.Service;

namespace TestCellHandshake.MqttService.MqttService.Workers
{
    public class TestCellWorker : BackgroundService
    {
        private readonly ILogger<TestCellWorker> _logger;
        private readonly IMqttService _mqttService;
        private readonly ITestCellChannel _testCellChannel;

        public TestCellWorker(ILogger<TestCellWorker> logger,
            IMqttService mqttService,
            ITestCellChannel testCellChannel)
        {
            _logger = logger;
            _mqttService = mqttService;
            _testCellChannel = testCellChannel;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting TestCellWorker");

            if (_mqttService.IsConnected())
            {
                _logger.LogInformation("Mqtt Service is connected from the test cell worker");
            }

            // Read from channel and publish mqtt message
            await foreach (var message in _testCellChannel.ReadAllAsync())
            {
                _logger.LogInformation("Backgroundworker: {service} received message of type {type}.", nameof(TestCellWorker), message.GetType().Name);

                Task messageTask = message switch
                {
                    ReqNewDataCommand => PublishReqNewData(message as ReqNewDataCommand),
                    ScannedDataCommand => PublishScannedData(message as ScannedDataCommand),
                    _ => throw new NotImplementedException()

                };

                await messageTask;
            }
        }

        private Task PublishReqNewData(ReqNewDataCommand? reqNewDataCommand)
        {
            ArgumentNullException.ThrowIfNull(reqNewDataCommand);
            var payload = reqNewDataCommand.ReqNewData.ToString().ToLower();
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/Cell/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData\"";
            string payloadKepwareFormat = $"[{{\"id\": {tagAddress},\"v\": {payload}}}]";

            return _mqttService.PublishAsync(topic, payloadKepwareFormat);
        }

        private Task PublishScannedData(ScannedDataCommand? scannedDataCommand)
        {
            ArgumentNullException.ThrowIfNull(scannedDataCommand);
            var payload = JsonSerializer.Serialize(scannedDataCommand.ScannedData.ToString());
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/Cell/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData\"";
            string payloadKepwareFormat = $"[{{\"id\": {tagAddress},\"v\": {payload}}}]";

            return _mqttService.PublishAsync(topic, payloadKepwareFormat);
        }
    }
}
