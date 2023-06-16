using Microsoft.AspNetCore.Mvc;
using TestCellHandshake.ApplicationLogic.Channels.Commands.LineController;
using TestCellHandshake.ApplicationLogic.Channels.Commands.TestCell;
using TestCellHandshake.MqttService.Channels;

namespace TestCellHandshake.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestCellController
    {
        private readonly ILogger<TestCellController> _logger;
        private readonly IMainCommandChannel _mainCommandChannel;

        public TestCellController(ILogger<TestCellController> logger, IMainCommandChannel mainCommandChannel)
        {
            _logger = logger;
            _mainCommandChannel = mainCommandChannel;
        }


        [Route("ReqNewDataCommand")]
        [HttpPost]
        public async Task ReqNewData(ReqNewDataCommand command)
        {
            _logger.LogInformation("ReqNewDataCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainCommandChannel));
            await _mainCommandChannel.AddCommandAsync(command);
        }


        [Route("ScannedDataCommand")]
        [HttpPost]
        public async Task ScannedData(ScannedDataCommand command)
        {
            _logger.LogInformation("ScannedDataCommand {command} added to channel: {channel}", command.ToString(), nameof(_mainCommandChannel));
            await _mainCommandChannel.AddCommandAsync(command);
        }

        [Route("ResetTestCell")]
        [HttpPost]
        public async Task ResetTestCell()
        {
            await _mainCommandChannel.AddCommandAsync(new ResetTestCellCommand());
        }
    }
}
