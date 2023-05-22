using Microsoft.AspNetCore.Mvc;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Commands.LineController;

namespace TestCellHandshake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestCellHandshakeController : ControllerBase
    {

        private readonly ILogger<TestCellHandshakeController> _logger;
        private readonly IMainCommandChannel _mainCommandChannel;

        public TestCellHandshakeController(ILogger<TestCellHandshakeController> logger, IMainCommandChannel mainCommandChannel)
        {
            _logger = logger;
            _mainCommandChannel = mainCommandChannel;
        }


        [Route("/DeviceIdCommand")]
        [HttpPost]
        public async Task DeviceIdCommand(DeviceIdCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("/DeviceTypeCommand")]
        [HttpPost]
        public async Task DeviceType(DeviceTypeCommand command)
        {
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("/DeviceDestinationCommand")]
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
    }
}
