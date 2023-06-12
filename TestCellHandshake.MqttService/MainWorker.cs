using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Channels.LineController;
using TestCellHandshake.MqttService.Channels.TestCell;
using TestCellHandshake.MqttService.Commands.LineController;
using TestCellHandshake.MqttService.Commands.TestCell;

namespace TestCellHandshake.MqttService
{
    public class MainWorker : BackgroundService
    {
        private readonly ILogger<MainWorker> _logger;
        private readonly IMainCommandChannel _mainCommandChannel;
        private readonly IMainMqttCommandChannel _mainMqttCommandChannel;
        private readonly ITestCellChannel _testCellChannel;

        public MainWorker(ILogger<MainWorker> logger,
            IMainCommandChannel mainCommandChannel,
            IMainMqttCommandChannel mainMqttCommandChannel,
            ITestCellChannel testCellChannel)
        {
            _logger = logger;
            _mainCommandChannel = mainCommandChannel;
            _mainMqttCommandChannel = mainMqttCommandChannel;
            _testCellChannel = testCellChannel;
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
                    ReqNewDataCommand => ReqNewDataMqttCommandHandler(message as ReqNewDataCommand),
                    ScannedDataCommand => ScannedDataMqttCommandHandler(message as ScannedDataCommand),
                    ResetCommand => ResetCommandHandler(message as ResetCommand),
                    _ => throw new NotImplementedException()
                };
            }
        }

        private async Task ResetCommandHandler(ResetCommand? resetCommand)
        {
            ArgumentNullException.ThrowIfNull(resetCommand);
            _logger.LogInformation("ResetCommand {command} added to channel: {channel} and {otherChannel}", resetCommand.ToString(), nameof(_testCellChannel), nameof(_mainMqttCommandChannel));
            await _testCellChannel.AddCommandAsync(resetCommand);
            await _mainMqttCommandChannel.AddCommandAsync(resetCommand);
        }

        private Task ScannedDataMqttCommandHandler(ScannedDataCommand? scannedDataCommand)
        {
            ArgumentNullException.ThrowIfNull(scannedDataCommand);
            _logger.LogInformation("ScannedDataCommand {command} added to channel: {channel}", scannedDataCommand.ToString(), nameof(_testCellChannel));
            _testCellChannel.AddCommandAsync(scannedDataCommand);
            return Task.CompletedTask;
        }

        private Task ReqNewDataMqttCommandHandler(ReqNewDataCommand? reqNewDataCommand)
        {
            ArgumentNullException.ThrowIfNull(reqNewDataCommand);
            _logger.LogInformation("ReqNewDataCommand {command} added to channel: {channel}", reqNewDataCommand.ToString(), nameof(_testCellChannel));
            _testCellChannel.AddCommandAsync(reqNewDataCommand);
            return Task.CompletedTask;
        }

        private Task NewDataRecMqttCommandHandler(NewDataRecCommand? newDataRecCommand)
        {
            ArgumentNullException.ThrowIfNull(newDataRecCommand);
            _logger.LogInformation("NewDataRecCommand {command} added to channel: {channel}", newDataRecCommand.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(newDataRecCommand);
            return Task.CompletedTask;
        }

        private Task DeviceDestinationMqttCommandHandler(DeviceDestinationCommand? deviceDestinationCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceDestinationCommand);
            _logger.LogInformation("DeviceDestinationCommand {command} added to channel: {channel}", deviceDestinationCommand.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(deviceDestinationCommand);
            return Task.CompletedTask;
        }

        private Task DeviceTypeMqttCommandHandler(DeviceTypeCommand? deviceTypeCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceTypeCommand);
            _logger.LogInformation("DeviceTypeCommand {command} added to channel: {channel}", deviceTypeCommand.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(deviceTypeCommand);
            return Task.CompletedTask;
        }

        private Task DeviceIdMqttCommandHandler(DeviceIdCommand? deviceIdCommand)
        {
            ArgumentNullException.ThrowIfNull(deviceIdCommand);
            _logger.LogInformation("DeviceIdCommand {command} added to channel: {channel}", deviceIdCommand.ToString(), nameof(_mainMqttCommandChannel));
            _mainMqttCommandChannel.AddCommandAsync(deviceIdCommand);
            return Task.CompletedTask;
        }
    }
}
