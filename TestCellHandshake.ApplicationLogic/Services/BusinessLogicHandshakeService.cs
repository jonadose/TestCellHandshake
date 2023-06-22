using TestCellHandshake.ApplicationLogic.Channels.Commands.LineController;
using TestCellHandshake.ApplicationLogic.Channels.Requests;
using TestCellHandshake.ApplicationLogic.Channels.ResponseChannel;
using TestCellHandshake.ApplicationLogic.Services.Models;

namespace TestCellHandshake.ApplicationLogic.Services
{
    public class BusinessLogicHandshakeService : IBusinessLogicHandshakeService
    {
        private readonly IHandshakeResponseChannel _handshakeResponseChannel;
        private readonly IDeviceDestinationService _deviceDestinationService;

        public BusinessLogicHandshakeService(IHandshakeResponseChannel handshakeResponseChannel,
            IDeviceDestinationService deviceDestinationService)
        {
            _handshakeResponseChannel = handshakeResponseChannel;
            _deviceDestinationService = deviceDestinationService;
        }


        public async Task HandleHandshakeRequest(HandshakeRequest? handshakeRequest)
        {
            // Receive the PowerUnitID from the handshake request
            var powerUnitId = handshakeRequest.PowerUnitId;
            var newDataRecValue = false;

            // Retrieve Data for the PowerUnitID from ME 
            PowerUnit powerunitFromMes = _deviceDestinationService.GetPowerUnit(powerUnitId);


            if (powerunitFromMes is not null)
            {
                newDataRecValue = true;
            }

            // Set the values to the corresponding commands 
            DeviceDestinationCommand deviceDestinationCommand = new() { DeviceDest = powerunitFromMes.DeviceDestination };
            DeviceIdCommand deviceIdCommand = new() { DeviceID = powerunitFromMes.DeviceID };
            DeviceTypeCommand deviceTypeCommand = new() { DeviceType = powerunitFromMes.DeviceType };
            NewDataRecCommand newDataRecCommand = new() { NewDataRec = newDataRecValue };


            // Add all commands to the response channel
            await _handshakeResponseChannel.AddCommandAsync(deviceTypeCommand);
            await _handshakeResponseChannel.AddCommandAsync(deviceIdCommand);
            await _handshakeResponseChannel.AddCommandAsync(deviceDestinationCommand);
            await _handshakeResponseChannel.AddCommandAsync(newDataRecCommand);
        }


        public async Task HandleCompleteHandshakeRequest(CompleteHandshakeRequest? completeHandshakeRequest)
        {
            NewDataRecCommand newDataRecCommand = new() { NewDataRec = false };
            await _handshakeResponseChannel.AddCommandAsync(newDataRecCommand);
            throw new NotImplementedException();
        }

    }
}
