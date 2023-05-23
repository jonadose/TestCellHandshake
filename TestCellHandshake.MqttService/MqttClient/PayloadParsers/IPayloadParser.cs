namespace TestCellHandshake.MqttService.MqttClient.PayloadParsers
{
    public interface IPayloadParser
    {
        ParsedPayload ParsePayloadSegment(ReadOnlyMemory<byte> payloadSegment);
    }
}
