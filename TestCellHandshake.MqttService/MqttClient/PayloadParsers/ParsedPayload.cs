using System.Text.Json;

namespace TestCellHandshake.MqttService.MqttClient.PayloadParsers
{
    public class ParsedPayload
    {
        public JsonElement TagAddress { get; set; }
        public JsonElement Value { get; set; }
    }
}
