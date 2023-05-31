using MQTTnet.Client;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public interface ILogicHandlingService
    {

        void HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs);
    }
}
