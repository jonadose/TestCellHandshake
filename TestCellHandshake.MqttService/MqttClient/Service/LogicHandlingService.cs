using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;
using TestCellHandshake.MqttService.Channels.LineController;
using TestCellHandshake.MqttService.Commands.LineController;
using TestCellHandshake.MqttService.MqttClient.ClientService;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service.Models;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class LogicHandlingService : ILogicHandlingService
    {
        private readonly ILogger<LogicHandlingService> _logger;
        private readonly IPayloadParser _payloadParser;
        private readonly IMainMqttCommandChannel _mainMqttCommandChannel;


        private const string _reqNewDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData";
        private const string _scannedDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData";

        private bool IsReqNewDataReady { get; set; } = false;
        private bool IsScannedDataReady { get; set; } = false;

        public LogicHandlingService(ILogger<LogicHandlingService> logger,
            IPayloadParser payloadParser,
            IMainMqttCommandChannel mainMqttCommandChannel)
        {
            _logger = logger;
            _payloadParser = payloadParser;
            _mainMqttCommandChannel = mainMqttCommandChannel;
        }


        public async Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payloadList = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);
            PowerUnit powerunit = HandlePayloadList(payloadList);

            if (powerunit is null)
            {
                _logger.LogInformation("Powerunit is null. Returning.");
            }
            else
            {
                ArgumentNullException.ThrowIfNull(powerunit);
                _logger.LogInformation("Powerunit is not null. Publishing to channel.");

                await PublishDeviceID(powerunit?.DeviceID);
                await PublishDeviceType(powerunit.DeviceType);
                await PublishDeviceDestination(powerunit.DeviceDestination);
                await PublishDeviceNewDataRec(powerunit.NewDataRec);
            }
        }


        private async Task PublishDeviceID(string? deviceID)
        {
            ArgumentNullException.ThrowIfNull(deviceID);
            DeviceIdCommand deviceIdCommand = new() { DeviceID = deviceID };
            await _mainMqttCommandChannel.AddCommandAsync(deviceIdCommand);
        }


        private async Task PublishDeviceType(int deviceType)
        {
            DeviceTypeCommand deviceTypeCommand = new() { DeviceType = deviceType };
            await _mainMqttCommandChannel.AddCommandAsync(deviceTypeCommand);
        }


        private async Task PublishDeviceDestination(int deviceDestination)
        {
            ArgumentNullException.ThrowIfNull(deviceDestination);
            DeviceDestinationCommand deviceDestinationCommand = new() { DeviceDest = deviceDestination };
            await _mainMqttCommandChannel.AddCommandAsync(deviceDestinationCommand);
        }


        private async Task PublishDeviceNewDataRec(bool newDataRec)
        {
            ArgumentNullException.ThrowIfNull(newDataRec);
            NewDataRecCommand newDataRecCommand = new() { NewDataRec = newDataRec };
            await _mainMqttCommandChannel.AddCommandAsync(newDataRecCommand);
        }


        private PowerUnit HandlePayloadList(List<ParsedPayload> parsedPayloadList)
        {
            PowerUnit powerUnit = new();

            foreach (var payload in parsedPayloadList)
            {
                _logger.LogInformation("TagAddress: {address}, value : {value}", payload.TagAddress, payload.Value);

                if (payload.TagAddress.ToString() == _reqNewDataTagAddress)
                {
                    _logger.LogInformation("TestCellHandshake tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                    IsReqNewDataReady = ConvertPayloadValueToBool(payload.Value);

                    // check if the scanned data tag is ready yet
                    if (IsScannedDataReady)
                    {
                        ResetControlFlags();
                        powerUnit = QueryMEforPowerunitData();
                    }
                    else
                    {
                        _logger.LogInformation("Scanned data tag not ready yet.");
                        return null;
                    }
                }
                if (payload.TagAddress.ToString() == _scannedDataTagAddress)
                {
                    _logger.LogInformation("Scanned data tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                    IsScannedDataReady = true;

                    // check if the reqNewdata tag is ready yet
                    if (IsReqNewDataReady)
                    {
                        ResetControlFlags();
                        powerUnit = QueryMEforPowerunitData();
                    }
                    else
                    {
                        _logger.LogInformation("ReqNewData tag not ready yet.");
                        return null;
                    }
                }
            }

            return powerUnit;
        }


        private bool ConvertPayloadValueToBool(JsonElement value)
        {
            bool payloadValueBool = false;

            if (value.ValueKind == JsonValueKind.True)
            {
                payloadValueBool = true;

            }
            if (value.ValueKind == JsonValueKind.False)
            {
                payloadValueBool = false;
            }

            return payloadValueBool;
        }


        private PowerUnit QueryMEforPowerunitData()
        {
            _logger.LogInformation("Querying ME for powerunit data.");

            PowerUnit powerunitFromMe = new()
            {
                DeviceID = "136gb1234DG0071234",
                DeviceType = 2,
                DeviceDestination = 3,
                NewDataRec = true
            };

            _logger.LogInformation("Powerunit data received from ME: {powerunitFromMe}", nameof(powerunitFromMe));
            _logger.LogInformation("DeviceID: {DeviceID} \n DeviceType: {DeviceType} \n DeviceDestination: {DeviceDestination} \n NewDataRec: {NewDataRec}",
                powerunitFromMe.DeviceID, powerunitFromMe.DeviceType, powerunitFromMe.DeviceDestination, powerunitFromMe.NewDataRec);
            return powerunitFromMe;
        }


        private void ResetControlFlags()
        {
            IsReqNewDataReady = false;
            IsScannedDataReady = false;
        }
    }
}
