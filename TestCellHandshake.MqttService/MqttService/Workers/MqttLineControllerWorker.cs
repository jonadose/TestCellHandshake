using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TestCellHandshake.MqttService.Channels.MqttLc;
using TestCellHandshake.MqttService.Commands;
using TestCellHandshake.MqttService.MqttService.Service;

namespace TestCellHandshake.MqttService.MqttService.Workers
{
    public class MqttLineControllerWorker : BackgroundService
    {
        private readonly ILogger<MqttLineControllerWorker> _logger;
        private readonly IMqttService _mqttService;
        private readonly IMainMqttCommandChannel _mainMqttCommandChannel;

        public MqttLineControllerWorker(ILogger<MqttLineControllerWorker> logger,
            IMqttService mqttService,
            IMainMqttCommandChannel mainMqttCommandChannel)
        {
            _logger = logger;
            _mqttService = mqttService;
            _mainMqttCommandChannel = mainMqttCommandChannel;
        }




        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting MqttLineControllerWorker");

            // Connect to mqtt broker 
            _mqttService.ConnectAsync();

            if (_mqttService.IsConnected())
            {
                _logger.LogInformation("MqttLineControllerWorker is connected");
            }

            // Read from internal channel and publish mqtt message 
            await foreach (var message in _mainMqttCommandChannel.ReadAllAsync())
            {
                _logger.LogInformation("Backgroundworker: {service} received message of type {type}.", nameof(MqttLineControllerWorker), message.GetType().Name);

                Task messageTask = message switch
                {
                    DeviceIdCommand => PublishDeviceId(message as DeviceIdCommand),
                    DeviceTypeCommand => PublishDeviceType(message as DeviceTypeCommand),
                    DeviceDestinationCommand => PublishDeviceDestination(message as DeviceDestinationCommand),
                    NewDataRecCommand => PublishNewDataRec(message as NewDataRecCommand),
                    _ => throw new NotImplementedException()
                };
            }

            // Wait forever to stop background service and then clear out the list
            await Task.Delay(Timeout.Infinite, stoppingToken);
            await _mqttService.UnsubscribeAsync("");
        }

        private Task PublishNewDataRec(NewDataRecCommand? newDataRecCommand)
        {
            ArgumentNullException.ThrowIfNull(newDataRecCommand);
            _mqttService.PublishAsync("Mcc/NewDataRec", $"{newDataRecCommand.NewDataRec}");
            return Task.CompletedTask;
        }

        private Task PublishDeviceDestination(DeviceDestinationCommand? deviceDestinationCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceDestinationCommand);
            _mqttService.PublishAsync("Mcc/DeviceDestination", $"{deviceDestinationCommand}");
            return Task.CompletedTask;
        }

        private Task PublishDeviceType(DeviceTypeCommand? deviceTypeCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceTypeCommand);
            _mqttService.PublishAsync("Mcc/DeviceType", $"{deviceTypeCommand}");
            return Task.CompletedTask;
        }

        private Task PublishDeviceId(DeviceIdCommand? deviceIdCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceIdCommand);
            var payload = JsonSerializer.Serialize(deviceIdCommand);
            _mqttService.PublishAsync("Mcc/DeviceId", $"{payload}");
            return Task.CompletedTask;
        }
    }
}
