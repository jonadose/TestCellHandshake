using Microsoft.AspNetCore.Mvc;
using TestCellHandshake.ApplicationLogic.Channels.Commands.LineController;
using TestCellHandshake.MqttService.Channels;

namespace TestCellHandshake.Web.Controllers
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


        [Route("ResetLineController")]
        [HttpPost]
        public async Task ResetLineController()
        {
            await _mainCommandChannel.AddCommandAsync(new ResetLineControllerCommand());
        }
    }
}
