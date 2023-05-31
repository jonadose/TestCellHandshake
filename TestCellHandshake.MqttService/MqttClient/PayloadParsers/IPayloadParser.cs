namespace TestCellHandshake.MqttService.MqttClient.PayloadParsers
{
    public interface IPayloadParser
    {
        List<ParsedPayload> ParsePayloadSegment(ReadOnlyMemory<byte> payloadSegment);
    }
}
