﻿using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using TestCellHandshake.ApplicationLogic.Channels.Commands.LineController;
using TestCellHandshake.ApplicationLogic.Channels.RequestChannel;
using TestCellHandshake.ApplicationLogic.Channels.Requests;
using TestCellHandshake.ApplicationLogic.Channels.ResponseChannel;
using TestCellHandshake.MqttService.MqttClient.PayloadParsers;
using TestCellHandshake.MqttService.MqttClient.Service.Models;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class LogicHandlingService : ILogicHandlingService
    {
        private string _handshakeRequestValue;
        private HandshakeRequest _handshakeRequest;

        private readonly ILogger<LogicHandlingService> _logger;
        private readonly IPayloadParser _payloadParser;
        private readonly IHandshakeRequestChannel _handshakeRequestChannel;
        private readonly IDeviceDestinationService _deviceDestinationService;


        private const string _reqNewDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ReqNewData";
        private const string _scannedDataTagAddress = "TestCell.Tester.PLC.DataBlocksGlobal.DataLC.Cell.Data.ScannedData";

        private bool IsReqNewDataReady { get; set; } = false;
        private bool IsScannedDataReady { get; set; } = false;
        private bool IsHandshakeInProgress { get; set; } = false;
        private bool IsHandshakeAborted { get; set; } = false;


        public LogicHandlingService(ILogger<LogicHandlingService> logger,
            IPayloadParser payloadParser,
            IHandshakeRequestChannel handshakeRequestChannel,
            IDeviceDestinationService deviceDestinationService)
        {
            _logger = logger;
            _payloadParser = payloadParser;
            _handshakeRequestChannel = handshakeRequestChannel;
            _deviceDestinationService = deviceDestinationService;
        }


        public async Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payloadList = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);


            if (IsHandshakeInProgress)
            {
                if (IsHandshakeAborted)
                {
                    IsHandshakeInProgress = false;
                    IsHandshakeAborted = false;
                    _logger.LogInformation("Handshake aborted. Returning.");

                }
                else
                {
                    await HandlePayloadHandshakeInProgress(payloadList);
                }
            }
            else
            {
                await RespondToHandshakeRequest(payloadList);
            }
        }


        private async Task RespondToHandshakeRequest(List<ParsedPayload> payloadList)
        {
            _handshakeRequest = HandlePayloadList(payloadList);

            if (_handshakeRequest is null)
            {
                _logger.LogInformation("Handshake request is null. Returning.");
            }
            else
            {
                _logger.LogInformation("Handshake request with value: {value}. Publishing to channel.", _handshakeRequest.PowerUnitId);
                await _handshakeRequestChannel.AddCommandAsync(_handshakeRequest);

                // THis logic will be moved to the NodeVikingRobotCellProcessor into the application layer
                //await PublishDeviceType(_powerUnit.DeviceType);
                //await PublishDeviceDestination(_powerUnit.DeviceDestination);
                //await PublishDeviceNewDataRec(_powerUnit.NewDataRec);

                // clear the _handshakeRequest
                _handshakeRequest = null;
            }
        }


        public void ResetHandshake()
        {
            IsHandshakeInProgress = false;
            IsHandshakeAborted = true;
        }


        //private async Task PublishDeviceType(int deviceType)
        //{
        //    DeviceTypeCommand deviceTypeCommand = new() { DeviceType = deviceType };
        //    await _handshakeRequestChannel.AddCommandAsync(deviceTypeCommand);
        //}


        //private async Task PublishDeviceDestination(int deviceDestination)
        //{
        //    ArgumentNullException.ThrowIfNull(deviceDestination);
        //    DeviceDestinationCommand deviceDestinationCommand = new() { DeviceDest = deviceDestination };
        //    await _handshakeRequestChannel.AddCommandAsync(deviceDestinationCommand);
        //}


        //private async Task PublishDeviceNewDataRec(bool newDataRec)
        //{
        //    ArgumentNullException.ThrowIfNull(newDataRec);
        //    NewDataRecCommand newDataRecCommand = new() { NewDataRec = newDataRec };
        //    await _handshakeRequestChannel.AddCommandAsync(newDataRecCommand);
        //}


        private HandshakeRequest HandlePayloadList(List<ParsedPayload> parsedPayloadList)
        {

            foreach (var payload in parsedPayloadList)
            {
                _logger.LogInformation("TagAddress: {address}, value : {value}", payload.TagAddress, payload.Value);
                var reqNewDataValue = ConvertPayloadValueToBool(payload.Value);

                if (payload.TagAddress.ToString() == _reqNewDataTagAddress && reqNewDataValue == true)
                {
                    _logger.LogInformation("TestCellHandshake tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                    IsReqNewDataReady = reqNewDataValue;

                    // check if the scanned data tag is ready yet
                    if (IsScannedDataReady)
                    {
                        ResetControlFlags();
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
                }

                if (payload.TagAddress.ToString() == _scannedDataTagAddress && payload.Value.ToString().Length > 1)
                {
                    _logger.LogInformation("Scanned data tag {address} found. Value: {value}", payload.TagAddress, payload.Value);
                    IsScannedDataReady = true;

                    // check if the reqNewdata tag is ready yet
                    if (IsReqNewDataReady)
                    {
                        ResetControlFlags();
                        _handshakeRequestValue = payload.Value.ToString();  // QueryMEforPowerunitData();
                        IsHandshakeInProgress = true;
                    }
                    else
                    {
                        _logger.LogInformation("ReqNewData tag not ready yet.");
                    }
                }
            }

            HandshakeRequest handshakeRequest = new() { PowerUnitId = _handshakeRequestValue };
            return handshakeRequest;
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
                    IsHandshakeInProgress = false;
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


        private PowerUnit QueryMEforPowerunitData()
        {
            return _deviceDestinationService.GetPowerUnit();
        }


        private void ResetControlFlags()
        {
            IsReqNewDataReady = false;
            IsScannedDataReady = false;
        }
    }
}
