using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class LogicHandlingService : ILogicHandlingService
    {
        private readonly ILogger<LogicHandlingService> _logger;
        private readonly IPayloadParser _payloadParser;

        private const string _reqNewDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData";
        private const string _scannedDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData";

        private bool IsReqNewDataReady { get; set; } = false;
        private bool IsScannedDataReady { get; set; } = false;

        public LogicHandlingService(ILogger<LogicHandlingService> logger,
            IPayloadParser payloadParser)
        {
            _logger = logger;
            _payloadParser = payloadParser;
        }


        public void HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payloadList = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);
            HandlePayloadList(payloadList);
        }


        private void HandlePayloadList(List<ParsedPayload> parsedPayloadList)
        {
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
                        QueryMEforPowerunitData();
                    }
                    else
                    {
                        _logger.LogInformation("Scanned data tag not ready yet.");
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
                        QueryMEforPowerunitData();
                    }
                    else
                    {
                        _logger.LogInformation("ReqNewData tag not ready yet.");
                    }
                }
            }
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


        private void QueryMEforPowerunitData()
        {
            _logger.LogInformation("Querying ME for powerunit data.");
        }


        private void ResetControlFlags()
        {
            IsReqNewDataReady = false;
            IsScannedDataReady = false;
        }
    }
}
