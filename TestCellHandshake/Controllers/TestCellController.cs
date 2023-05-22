using Microsoft.AspNetCore.Mvc;
using TestCellHandshake.MqttService.Channels;
using TestCellHandshake.MqttService.Commands.TestCell;

namespace TestCellHandshake.Controllers
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
    }
}
