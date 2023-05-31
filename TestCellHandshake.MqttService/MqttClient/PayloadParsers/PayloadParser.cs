using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace TestCellHandshake.MqttService.MqttClient.PayloadParsers
{
    public class PayloadParser : IPayloadParser
    {
        private readonly ILogger<PayloadParser> _logger;
        public PayloadParser(ILogger<PayloadParser> logger)
        {
            _logger = logger;
        }

        public List<ParsedPayload> ParsePayloadSegment(ReadOnlyMemory<byte> payloadSegment)
        {
            string payloadString = Encoding.UTF8.GetString(payloadSegment.Span);

            JsonElement parsedJson = JsonDocument.Parse(payloadString).RootElement;
            ParsedPayload parsedPayload = new();
            List<ParsedPayload> parsedPayloadList = new();

            try
            {
                var values = parsedJson.GetProperty("values");

                // check that values is an array and not null 
                if (values.ValueKind != JsonValueKind.Array && values.GetArrayLength() > 0)
                {
                    _logger.LogError("Unable to parse payload. Values is not an array.");
                    throw new Exception("Unable to parse payload. Values is not an array.");
                }

                // The array can potentially contain multiple elements and there is no telling how many
                foreach (var value in values.EnumerateArray())
                {
                    parsedPayload.TagAddress = value.GetProperty("id");
                    parsedPayload.Value = value.GetProperty("v");

                    parsedPayloadList.Add(parsedPayload);
                }

                return parsedPayloadList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to parse paylaod. Error message. {message}.", ex.Message);
                throw;
            }
        }
    }
}
