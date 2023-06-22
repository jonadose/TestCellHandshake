using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using TestCellHandshake.ApplicationLogic.Channels.RequestChannel;
using TestCellHandshake.ApplicationLogic.Channels.Requests;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class LogicHandlingService : ILogicHandlingService
    {
        private string _handshakeRequestValue;
        private HandshakeRequest _handshakeRequest;

        private readonly ILogger<LogicHandlingService> _logger;
        private readonly IPayloadParser _payloadParser;
        private readonly IHandshakeRequestChannel _handshakeRequestChannel;


        private const string _reqNewDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData";
        private const string _scannedDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData";

        private bool IsReqNewDataReady { get; set; } = false;
        private bool AreReqNewDataChecksPassed { get; set; } = false;
        private bool AreScannedDataChecksPassed { get; set; } = false;
        private bool IsScannedDataReady { get; set; } = false;
        private bool IsHandshakeInProgress { get; set; } = false;
        private bool IsHandshakeAborted { get; set; } = false;


        public LogicHandlingService(ILogger<LogicHandlingService> logger,
            IPayloadParser payloadParser,
            IHandshakeRequestChannel handshakeRequestChannel)
        {
            _logger = logger;
            _payloadParser = payloadParser;
            _handshakeRequestChannel = handshakeRequestChannel;
        }


        public async Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payloadList = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);


            if (IsHandshakeAborted)
            {
                IsHandshakeInProgress = false;
                IsHandshakeAborted = false;
                _logger.LogInformation("Handshake aborted. Returning.");
            }
            else if (IsHandshakeInProgress)
            {
                await HandlePayloadHandshakeInProgress(payloadList);

            }
            else
            {
                await RespondToHandshakeRequest(payloadList);

            }
        }


        private async Task RespondToHandshakeRequest(List<ParsedPayload> payloadList)
        {
            _handshakeRequest = HandlePayloadList(payloadList);

            if (_handshakeRequest is null || _handshakeRequest.PowerUnitId is null || !IsReqNewDataReady)
            {
                _logger.LogInformation("Handshake request is null. Returning.");
                return;
            }

            _logger.LogInformation("Handshake request with value: {value}. Publishing to channel.", _handshakeRequest.PowerUnitId);
            await _handshakeRequestChannel.AddCommandAsync(_handshakeRequest);

            // clear the _handshakeRequest
            _handshakeRequest = null;
        }


        private HandshakeRequest? HandlePayloadList(List<ParsedPayload> parsedPayloadList)
        {

            foreach (var payload in parsedPayloadList)
            {
                _logger.LogInformation("TagAddress: {address}, value : {value}", payload.TagAddress, payload.Value);

                if (payload.TagAddress.ToString() == _reqNewDataTagAddress)
                {
                    AreReqNewDataChecksPassed = ReqNewDataChecks(payload);
                }
                else if (payload.TagAddress.ToString() == _scannedDataTagAddress)
                {
                    AreScannedDataChecksPassed = ScannedDataChecks(payload);
                }
            }

            if (AreReqNewDataChecksPassed && AreScannedDataChecksPassed)
            {
                _logger.LogInformation("ReqNewDataChecks: {1} and ScannedDataChecks: {2}.", AreReqNewDataChecksPassed, AreScannedDataChecksPassed);
                HandshakeRequest handshakeRequest = new() { PowerUnitId = _handshakeRequestValue };
                return handshakeRequest;
            }
            else
            {
                _logger.LogInformation("ReqNewDataChecks: {1} and ScannedDataChecks: {2}.", AreReqNewDataChecksPassed, AreScannedDataChecksPassed);
                return null;
            }
        }


        private bool ReqNewDataChecks(ParsedPayload payload)
        {
            var reqNewDataValue = ConvertPayloadValueToBool(payload.Value);

            if (payload.TagAddress.ToString() == _reqNewDataTagAddress && reqNewDataValue == true)
            {
                _logger.LogInformation("TestCellHandshake tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                IsReqNewDataReady = reqNewDataValue;

                // check if the scanned data tag is ready yet
                if (IsScannedDataReady)
                {
                    _handshakeRequestValue = payload.Value.ToString();
                    IsHandshakeInProgress = true;
                }
                else
                {
                    _logger.LogInformation("Scanned data tag not ready yet.");
                }
            }
            else if (payload.TagAddress.ToString() == _reqNewDataTagAddress && reqNewDataValue == false)
            {
                _logger.LogInformation("{tag} is {value}", payload.TagAddress, payload.Value);
                IsReqNewDataReady = reqNewDataValue;
            }

            return IsReqNewDataReady;
        }


        private bool ScannedDataChecks(ParsedPayload payload)
        {
            if (payload.TagAddress.ToString() == _scannedDataTagAddress && payload.Value.ToString().Length > 1)
            {
                _logger.LogInformation("Scanned data tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                IsScannedDataReady = true;

                // check if the reqNewdata tag is ready yet
                if (IsReqNewDataReady)
                {
                    _handshakeRequestValue = payload.Value.ToString();  // QueryMEforPowerunitData();
                    IsHandshakeInProgress = true;
                }
                else
                {
                    _logger.LogInformation("ReqNewData tag not ready yet.");
                }
            }
            else if (payload.TagAddress.ToString() == _scannedDataTagAddress)
            {
                _logger.LogInformation("{tag} is {value}", payload.TagAddress, payload.Value);
                IsScannedDataReady = false;
            }

            return IsScannedDataReady;
        }


        private async Task HandlePayloadHandshakeInProgress(List<ParsedPayload> parsedPayloadList)
        {
            _logger.LogInformation("Handshake in progress.");

            foreach (var payload in parsedPayloadList)
            {
                if (payload.TagAddress.ToString() == _reqNewDataTagAddress)
                {
                    _logger.LogInformation("TestCellHandshake tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                    _logger.LogInformation("Will set the NewDataRec to false");

                    // Send a CompleteHanhakeRequest to the NodeVikingRobotCellProcessor
                    await _handshakeRequestChannel.AddCommandAsync(new CompleteHandshakeRequest()); // await PublishDeviceNewDataRec(false);

                    // Reset the HandshakeInProgress flag
                    ResetControlFlags();
                }
                else
                {
                    _logger.LogInformation("No action for this address: {address}.", payload.TagAddress);
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


        private void ResetControlFlags()
        {
            IsReqNewDataReady = false;
            IsScannedDataReady = false;
            IsHandshakeInProgress = false;
            AreReqNewDataChecksPassed = false;
            AreScannedDataChecksPassed = false;
        }


        public void ResetHandshake()
        {
            IsHandshakeInProgress = false;
            IsHandshakeAborted = true;
        }
    }
}
