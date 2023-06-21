using TestCellHandshake.ApplicationLogic.Services.Models;

namespace TestCellHandshake.ApplicationLogic.Services
{
    public interface IDeviceDestinationService
    {
        PowerUnit GetPowerUnit(string PowerUnitId);
    }
}
