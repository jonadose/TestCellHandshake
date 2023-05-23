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

        public ParsedPayload ParsePayloadSegment(ReadOnlyMemory<byte> payloadSegment)
        {
            string payloadString = Encoding.UTF8.GetString(payloadSegment.Span);

            JsonElement parsedJson = JsonDocument.Parse(payloadString).RootElement;
            ParsedPayload parsedPayload = new ParsedPayload();

            try
            {
                var values = parsedJson.GetProperty("values");

                // check that values is an array and not null 
                if (values.ValueKind != JsonValueKind.Array && values.GetArrayLength() > 0)
                {
                    _logger.LogError("Unable to parse payload. Values is not an array.");
                    throw new Exception("Unable to parse payload. Values is not an array.");
                }
                var value = values.EnumerateArray().First();
                parsedPayload.TagAddress = value.GetProperty("id");
                parsedPayload.Value = value.GetProperty("v");

                return parsedPayload;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to parse paylaod. Error message. {message}.", ex.Message);
                throw;
            }
        }
    }
}
