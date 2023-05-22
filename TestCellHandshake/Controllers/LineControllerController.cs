using Microsoft.AspNetCore.Mvc;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Commands;
using TestCellHandshake.MqttService.Commands.LineController;

namespace TestCellHandshake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LineControllerController : ControllerBase
    {

        private readonly ILogger<LineControllerController> _logger;
        private readonly IMainCommandChannel _mainCommandChannel;

        public LineControllerController(ILogger<LineControllerController> logger, IMainCommandChannel mainCommandChannel)
        {
            _logger = logger;
            _mainCommandChannel = mainCommandChannel;
        }


        [Route("DeviceIdCommand")]
        [HttpPost]
        public async Task DeviceIdCommand(DeviceIdCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("DeviceTypeCommand")]
        [HttpPost]
        public async Task DeviceType(DeviceTypeCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("DeviceDestinationCommand")]
        [HttpPost]
        public async Task DeviceDestination(DeviceDestinationCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("NewDataRecCommand")]
        [HttpPost]
        public async Task NewDataRec(NewDataRecCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("ResetAllTags")]
        [HttpPost]
        public async Task ResetAllTags()
        {
            var resetDeviceIdCommand = new DeviceIdCommand { DeviceID = "String 1" };
            var resetDeviceTypeCommand = new DeviceTypeCommand { DeviceType = 0 };
            var resetDeviceDestinationCommand = new DeviceDestinationCommand { DeviceDest = 0 };
            var resetNewDataRecCommand = new NewDataRecCommand { NewDataRec = false };

            List<BaseMainCommand> commandList = new()
            {
                resetDeviceIdCommand,
                resetDeviceTypeCommand,
                resetDeviceDestinationCommand,
                resetNewDataRecCommand
            };

            foreach (var command in commandList)
            {
                await _mainCommandChannel.AddCommandAsync(command);
            }
        }
    }
}
