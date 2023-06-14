using Microsoft.Extensions.Logging;
using TestCellHandshake.MqttService.MqttClient.Service.Models;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class DeviceDestinationService : IDeviceDestinationService
    {
        private readonly ILogger<DeviceDestinationService> _logger;

        public DeviceDestinationService(ILogger<DeviceDestinationService> logger)
        {
            _logger = logger;
        }


        public PowerUnit GetPowerUnit()
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
    }
}
