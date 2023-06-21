using Microsoft.Extensions.Logging;
using TestCellHandshake.ApplicationLogic.Services.Models;

namespace TestCellHandshake.ApplicationLogic.Services
{
    public class DeviceDestinationService : IDeviceDestinationService
    {
        private readonly ILogger<DeviceDestinationService> _logger;

        public DeviceDestinationService(ILogger<DeviceDestinationService> logger)
        {
            _logger = logger;
        }


        public PowerUnit GetPowerUnit(string PowerUnitId)
        {
            _logger.LogInformation("Querying ME for powerunit data.");

            PowerUnit powerunitFromMe = new()
            {
                DeviceID = PowerUnitId,
                DeviceType = 2,
                DeviceDestination = 3,
                NewDataRec = true
            };

            _logger.LogInformation("Powerunit data received from ME: {powerunitFromMe}", nameof(powerunitFromMe));
            _logger.LogInformation("DeviceID: {DeviceID} \n DeviceType: {DeviceType} \n DeviceDestination: {DeviceDestination} \n NewDataRec: {NewDataRec}",
                powerunitFromMe.DeviceID, powerunitFromMe.DeviceType, powerunitFromMe.DeviceDestination, powerunitFromMe.NewDataRec);
            return powerunitFromMe;
        }
    }
}
