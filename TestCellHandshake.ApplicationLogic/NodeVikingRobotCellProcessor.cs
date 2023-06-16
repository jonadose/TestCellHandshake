using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCellHandshake.ApplicationLogic.Channels.RequestChannel;
using TestCellHandshake.ApplicationLogic.Channels.Requests;
using TestCellHandshake.ApplicationLogic.Services;

namespace TestCellHandshake.ApplicationLogic
{
    public class NodeVikingRobotCellProcessor : BackgroundService
    {
        private readonly ILogger<NodeVikingRobotCellProcessor> _logger;
        private readonly IHandshakeRequestChannel _handshakeRequestChannel;
        private readonly IBusinessLogicHandshakeService _businessLogicHandshakeService;

        public NodeVikingRobotCellProcessor(ILogger<NodeVikingRobotCellProcessor> logger,
            IHandshakeRequestChannel handshakeRequestChannel,
            IBusinessLogicHandshakeService businessLogicHandshakeService)
        {
            _logger = logger;
            _handshakeRequestChannel = handshakeRequestChannel;
            _businessLogicHandshakeService = businessLogicHandshakeService;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting NodeVikingRobotCellProcessor");

            try
            {
                await foreach (var message in _handshakeRequestChannel.ReadAllAsync())
                {
                    _logger.LogInformation("Backgroundworker: {service} received message of type {type}.", nameof(NodeVikingRobotCellProcessor), message.GetType().Name);

                    Task messageTask = message switch
                    {
                        HandshakeRequest => _businessLogicHandshakeService.HandleHandshakeRequest(message as HandshakeRequest),
                        CompleteHandshakeRequest => _businessLogicHandshakeService.HandleCompleteHandshakeRequest(message as CompleteHandshakeRequest),
                        _ => throw new NotImplementedException()
                    };
                }

                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"NodeVikingRobotCellProcessor failed. Exception message: {ex.Message}");
            }
        }
    }
}
