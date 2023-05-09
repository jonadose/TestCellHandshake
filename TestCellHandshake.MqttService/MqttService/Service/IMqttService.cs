using MQTTnet.Client;

namespace TestCellHandshake.MqttService.MqttService.Service
{
    public interface IMqttService
    {
        void Initialize(MqttClientOptions mqttClientOptions);
        Task SubscribeAsync(string topic);
        Task UnsubscribeAsync(string topic);
        Task ConnectAsync();
        bool IsConnected();
        Task PublishAsync(string topic, string payload);
        void MethodThatTakesAction(Func<MqttApplicationMessageReceivedEventArgs, Task> action);
    }
}
