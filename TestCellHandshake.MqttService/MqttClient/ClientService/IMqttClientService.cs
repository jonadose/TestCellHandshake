using MQTTnet.Client;

namespace TestCellHandshake.MqttService.MqttClient.ClientService
{
    public interface IMqttClientService
    {
        Task ConnectAsync();
        Task SubscribeAsync(string topic);
        Task UnsubscribeAsync(string topic);
        void MethodThatTakesAction(Func<MqttApplicationMessageReceivedEventArgs, Task> func);
        Task HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs);
        bool IsConnected();
    }
}
