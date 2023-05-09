
namespace TestCellHandshake.MqttService.Commands
{
    public class DeviceIdCommand : BaseMainCommand
    {
        public required string SfcId { get; set; }
    }
}
