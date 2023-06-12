

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TestCellHandshake.MqttService.Channels.TestCell;
using TestCellHandshake.MqttService.Commands.LineController;
using TestCellHandshake.MqttService.Commands.TestCell;
using TestCellHandshake.MqttService.MqttClient.Service;
using TestCellHandshake.MqttService.MqttService.Service;

namespace TestCellHandshake.MqttService.MqttService.Workers
{
    public class TestCellWorker : BackgroundService
    {
        private readonly ILogger<TestCellWorker> _logger;
        private readonly IMqttService _mqttService;
        private readonly ITestCellChannel _testCellChannel;
        private readonly ILogicHandlingService _logicHandlingService;

        public TestCellWorker(ILogger<TestCellWorker> logger,
            IMqttService mqttService,
            ITestCellChannel testCellChannel,
            ILogicHandlingService logicHandlingService)
        {
            _logger = logger;
            _mqttService = mqttService;
            _testCellChannel = testCellChannel;
            _logicHandlingService = logicHandlingService;
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
                    ResetCommand => Reset(message as ResetCommand),
                    _ => throw new NotImplementedException()

                };

                await messageTask;
            }
        }

        private async Task Reset(ResetCommand? resetCommand)
        {

            // TODO: HIS SHOULD NOT BE HERE. IT INTRODUCES A WEIRD DEPENDENCY REOMOVE IN MCC 
            _logicHandlingService.ResetHandshake();

            // Reset ReqNewData
            var payload1 = "false";
            string topic1 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/Cell/Data";
            string tagAddress1 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData\"";
            string payloadKepwareFormat1 = $"[{{\"id\": {tagAddress1},\"v\": {payload1}}}]";

            await _mqttService.PublishAsync(topic1, payloadKepwareFormat1);

            await Task.Delay(100);

            // Reset ScannedData
            var payload2 = "0";
            string topic2 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/Cell/Data";
            string tagAddress2 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData\"";
            string payloadKepwareFormat2 = $"[{{\"id\": {tagAddress2},\"v\": {payload2}}}]";

            await _mqttService.PublishAsync(topic2, payloadKepwareFormat2);
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
