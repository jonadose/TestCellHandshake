using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using TestCellHandshake.ApplicationLogic.Channels.Commands.LineController;
using TestCellHandshake.ApplicationLogic.Channels.ResponseChannel;
using TestCellHandshake.MqttService.MqttClient.Service;
using TestCellHandshake.MqttService.MqttService.Service;

namespace TestCellHandshake.MqttService.MqttClient
{
    public class TestCellHandshakeProcessor : BackgroundService
    {
        private readonly ILogger<TestCellHandshakeProcessor> _logger;
        private readonly IMqttService _mqttService;
        private readonly IHandshakeResponseChannel _handshakeResponseChannel;
        private readonly ILogicHandlingService _logicHandlingService;

        public TestCellHandshakeProcessor(ILogger<TestCellHandshakeProcessor> logger,
            IMqttService mqttService,
            IHandshakeResponseChannel handshakeResponseChannel,
            ILogicHandlingService logicHandlingService)
        {
            _logger = logger;
            _mqttService = mqttService;
            _handshakeResponseChannel = handshakeResponseChannel;
            _logicHandlingService = logicHandlingService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting LineControllerClient");

            try
            {
                // Link the client service to the correct method to call when an event is triggered
                _mqttService.MethodThatTakesAction(MethodToHandleEvent);

                // Connect to mqtt broker
                await _mqttService.ConnectAsync();

                if (_mqttService.IsConnected())
                {
                    _logger.LogInformation("MqttClientService is connected");
                }

                // Subscribe to topic
                await _mqttService.SubscribeAsync("iotgateway/testcell");

                // Read from internal channel and publish mqtt message 
                await foreach (var message in _handshakeResponseChannel.ReadAllAsync())
                {
                    _logger.LogInformation("Backgroundworker: {service} received message of type {type}.", nameof(TestCellHandshakeProcessor), message.GetType().Name);

                    Task messageTask = message switch
                    {
                        DeviceIdCommand => PublishDeviceId(message as DeviceIdCommand),
                        DeviceTypeCommand => PublishDeviceType(message as DeviceTypeCommand),
                        DeviceDestinationCommand => PublishDeviceDestination(message as DeviceDestinationCommand),
                        NewDataRecCommand => PublishNewDataRec(message as NewDataRecCommand),
                        ResetLineControllerCommand => Reset(message as ResetLineControllerCommand),
                        _ => throw new NotImplementedException()
                    };
                }

                await Task.Delay(Timeout.Infinite, stoppingToken);
                await _mqttService.UnsubscribeAsync("iotgateway/testcell");
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

        private async Task Reset(ResetLineControllerCommand? resetCommand)
        {

            // TODO: HIS SHOULD NOT BE HERE. IT INTRODUCES A WEIRD DEPENDENCY REOMOVE IN MCC 
            _logicHandlingService.ResetHandshake();

            // Reset DeviceID 
            var payload1 = "0";
            string topic1 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress1 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceID\"";
            string payloadKepwareFormat1 = $"[{{ \"id\": {tagAddress1},\"v\": {payload1}}}]";

            await _mqttService.PublishAsync(topic1, payloadKepwareFormat1);

            // Reset DeviceType
            var payload2 = 0;
            string topic2 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress2 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceType\"";
            string payloadKepwareFormat2 = $"[{{ \"id\": {tagAddress2},\"v\": {payload2}}}]";

            await _mqttService.PublishAsync(topic2, payloadKepwareFormat2);

            // Reset DeviceDestination
            var payload3 = 0;
            string topic3 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress3 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceDest\"";
            string payloadKepwareFormat3 = $"[{{ \"id\": {tagAddress3},\"v\": {payload3}}}]";

            await _mqttService.PublishAsync(topic3, payloadKepwareFormat3);

            // Reset NewDataRec
            var payload4 = "false";
            string topic4 = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress4 = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.NewDataRec\"";
            string payloadKepwareFormat4 = $"[{{ \"id\": {tagAddress4},\"v\": {payload4}}}]";

            await _mqttService.PublishAsync(topic4, payloadKepwareFormat4);
        }

        private Task PublishNewDataRec(NewDataRecCommand? newDataRecCommand)
        {
            ArgumentNullException.ThrowIfNull(newDataRecCommand);
            var payload = newDataRecCommand.NewDataRec.ToString().ToLower();
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.NewDataRec\"";
            string payloadKepwareFormat = $"[{{ \"id\": {tagAddress},\"v\": {payload}}}]";

            _mqttService.PublishAsync(topic, payloadKepwareFormat);
            return Task.CompletedTask;
        }

        private Task PublishDeviceDestination(DeviceDestinationCommand? deviceDestinationCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceDestinationCommand);
            var payload = deviceDestinationCommand.DeviceDest;
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceDest\"";
            string payloadKepwareFormat = $"[{{ \"id\": {tagAddress},\"v\": {payload}}}]";

            _mqttService.PublishAsync(topic, payloadKepwareFormat);
            return Task.CompletedTask;
        }

        private Task PublishDeviceType(DeviceTypeCommand? deviceTypeCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceTypeCommand);
            var payload = deviceTypeCommand.DeviceType;
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceType\"";
            string payloadKepwareFormat = $"[{{ \"id\": {tagAddress},\"v\": {payload}}}]";

            _mqttService.PublishAsync(topic, payloadKepwareFormat);
            return Task.CompletedTask;
        }

        private Task PublishDeviceId(DeviceIdCommand? deviceIdCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceIdCommand);
            var payload = JsonSerializer.Serialize(deviceIdCommand.DeviceID.ToString());
            string topic = "TestCell/Tester/PLC/DataBlocksGlobal/DataLC/LC/Prg/Data";
            string tagAddress = "\"TestCell.Tester.PLC.DataBlocksGlobal.DataLC.LC.Prg.Data.DeviceID\"";
            string payloadKepwareFormat = $"[{{ \"id\": {tagAddress},\"v\": {payload}}}]";

            _mqttService.PublishAsync(topic, payloadKepwareFormat);
            return Task.CompletedTask;
        }
    }
}
