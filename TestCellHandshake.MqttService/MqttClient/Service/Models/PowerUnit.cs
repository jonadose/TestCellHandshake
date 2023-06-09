namespace TestCellHandshake.MqttService.MqttClient.Service.Models
{
    public class PowerUnit
    {
        public string? DeviceID { get; set; }
        public int DeviceType { get; set; }
        public int DeviceDestination { get; set; }
        public bool NewDataRec { get; set; }
    }
}
