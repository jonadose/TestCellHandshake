using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using TestCellHandshake.OpcuaService.Service;

namespace TestCellHandshake.OpcuaService.Worker
{
    public class OpcuaTestCellWorker : BackgroundService
    {
        private readonly ILogger<OpcuaTestCellWorker> _logger;
        private readonly IOpcuaService _opcuaService;

        public OpcuaTestCellWorker(ILogger<OpcuaTestCellWorker> logger, IOpcuaService opcuaService)
        {
            _logger = logger;
            _opcuaService = opcuaService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OpcuaTestCellWorker started");

            // Connect OpcuService to Kepware 
            try
            {
                _opcuaService.InitializeClient();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to connect opcua client: {ex}.", ex);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                string topic = "Hestegård";
                string paylaod = "Hest";
                _opcuaService.Publish(topic, paylaod);
                await Task.Delay(1_000, stoppingToken);
            }
        }
    }
}