
namespace TestCellHandshake.MqttService.Commands
{
    public class DeviceDestinationCommand : BaseMainCommand
    {
        public required int Destination { get; set; }
    }
}
