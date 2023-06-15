using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestCellHandshake.ApplicationLogic
{
    public class NodeVikingRobotCellProcessor : BackgroundService
    {
        private readonly ILogger<NodeVikingRobotCellProcessor> _logger;

        public NodeVikingRobotCellProcessor(ILogger<NodeVikingRobotCellProcessor> logger)
        {
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting NodeVikingRobotCellProcessor");

            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError($"NodeVikingRobotCellProcessor failed. Exception message: {ex.Message}");
            }
        }
    }
}
