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


        public Task HandleHandshakeRequest(HandshakeRequest? handshakeRequest)
        {
            // Receive the PowerUnitID from the handshake request
            var powerUnitId = handshakeRequest.PowerUnitId;

            // Retrieve Data for the PowerUnitID from ME 
            // TODO: This implementation will be MesService in MCC
            PowerUnit powerunitFromMes = _deviceDestinationService.GetPowerUnit(powerUnitId);

            // Set the values to the corresponding commands 
            DeviceDestinationCommand deviceDestinationCommand = new() { DeviceDest = powerunitFromMes.DeviceDestination };
            DeviceIdCommand deviceIdCommand = new() { DeviceID = powerunitFromMes.DeviceID };
            DeviceTypeCommand deviceTypeCommand = new() { DeviceType = powerunitFromMes.DeviceType };
            NewDataRecCommand newDataRecCommand = new() { NewDataRec = powerunitFromMes.NewDataRec };


            // Add all commands to the response channel
            _handshakeResponseChannel.AddCommandAsync(deviceTypeCommand);
            _handshakeResponseChannel.AddCommandAsync(deviceIdCommand);
            _handshakeResponseChannel.AddCommandAsync(deviceDestinationCommand);
            _handshakeResponseChannel.AddCommandAsync(newDataRecCommand);

            return Task.CompletedTask;
        }


        public async Task HandleCompleteHandshakeRequest(CompleteHandshakeRequest? completeHandshakeRequest)
        {
            // THis should do what // await PublishDeviceNewDataRec(false); did in logic handling service

            //private async Task PublishDeviceNewDataRec(bool newDataRec)
            //{
            //    ArgumentNullException.ThrowIfNull(newDataRec);
            //    NewDataRecCommand newDataRecCommand = new() { NewDataRec = newDataRec };
            //    await _handshakeRequestChannel.AddCommandAsync(newDataRecCommand);
            //}

            NewDataRecCommand newDataRecCommand = new() { NewDataRec = false };
            await _handshakeResponseChannel.AddCommandAsync(newDataRecCommand);
            throw new NotImplementedException();
        }

    }
}
