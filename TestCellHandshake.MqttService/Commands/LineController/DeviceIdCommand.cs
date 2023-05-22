namespace TestCellHandshake.MqttService.Commands.LineController
{
    public class DeviceIdCommand : BaseMainCommand
    {
        public required string DeviceID { get; set; }
    }
}
