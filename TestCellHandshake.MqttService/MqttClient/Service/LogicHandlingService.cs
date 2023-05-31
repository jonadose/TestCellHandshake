using MQTTnet.Client;
using System.Text;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public class LogicHandlingService
    {
        public LogicHandlingService()
        {

        }


        public void HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss.fff");
            _logger.LogInformation($"Received application message from topic: {eventArgs.ApplicationMessage.Topic}. Timestamp: {formattedTime}.");
            _logger.LogInformation("Payload: " + Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment));

            var payloadList = _payloadParser.ParsePayloadSegment(eventArgs.ApplicationMessage.PayloadSegment);

            foreach (var payload in payloadList)
            {
                _logger.LogInformation("Parsed payload.TagAddress: {address}, value : {value}", payload.TagAddress, payload.Value);
            }
        }
    }
}
