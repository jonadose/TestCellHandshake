using MQTTnet.Client;

namespace TestCellHandshake.MqttService.MqttClient.Service
{
    public interface ILogicHandlingService
    {

        Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs);
    }
}
