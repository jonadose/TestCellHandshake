using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Channels.MqttLc;
using TestCellHandshake.MqttService.Commands;

namespace TestCellHandshake.MqttService
{
    public class MainWorker : BackgroundService
    {
        private readonly ILogger<MainWorker> _logger;
        private readonly IMainCommandChannel _mainCommandChannel;
        private readonly IMainMqttCommandChannel _mainMqttCommandChannel;

        public MainWorker(ILogger<MainWorker> logger, IMainCommandChannel mainCommandChannel, IMainMqttCommandChannel mainMqttCommandChannel)
        {
            _logger = logger;
            _mainCommandChannel = mainCommandChannel;
            _mainMqttCommandChannel = mainMqttCommandChannel;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainWorker started");

            await foreach (var message in _mainCommandChannel.ReadAllAsync())
            {
                _logger.LogInformation("{serviceName} received message of type {type}", nameof(MainWorker), message.GetType().Name);
                Task messageTask = message switch
                {
                    DeviceIdCommand => DeviceIdMqttCommandHandler(message as DeviceIdCommand),
                    DeviceTypeCommand => DeviceTypeMqttCommandHandler(message as DeviceTypeCommand),
                    DeviceDestinationCommand => DeviceDestinationMqttCommandHandler(message as DeviceDestinationCommand),
                    NewDataRecCommand => NewDataRecMqttCommandHandler(message as NewDataRecCommand),
                    _ => throw new NotImplementedException()
                };
            }
        }

        private Task NewDataRecMqttCommandHandler(NewDataRecCommand command)
        {
            _logger.LogInformation("NewDataRecCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(command);
            return Task.CompletedTask;
        }

        private Task DeviceDestinationMqttCommandHandler(DeviceDestinationCommand command)
        {
            _logger.LogInformation("DeviceDestinationCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(command);
            return Task.CompletedTask;
        }

        private Task DeviceTypeMqttCommandHandler(DeviceTypeCommand command)
        {
            _logger.LogInformation("DeviceTypeCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(command);
            return Task.CompletedTask;
        }

        private Task DeviceIdMqttCommandHandler(DeviceIdCommand command)
        {
            _logger.LogInformation("DeviceIdCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(command);
            return Task.CompletedTask;
        }
    }
}
