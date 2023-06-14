using TestCellHandshake.MqttService.MqttClient.Service.Models;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public interface IDeviceDestinationService
    {
        PowerUnit GetPowerUnit();
    }
}
