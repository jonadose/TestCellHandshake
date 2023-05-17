using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.UaFx.Client;
using TestCellHandshake.OpcuaService.Configuration;

namespace TestCellHandshake.OpcuaService.Service
{
    public class OpcuaService : IOpcuaService
    {
        private OpcClient? _opcClient;
        private readonly ILogger<OpcuaService> _logger;
        private readonly IOptionsMonitor<OpcuaConfig> _opcuaConfig;

        public OpcuaService(ILogger<OpcuaService> logger, IOptionsMonitor<OpcuaConfig> opcuaConfig)
        {
            _logger = logger;
            _opcuaConfig = opcuaConfig;
            InitializeClient();
        }



        public void InitializeClient()
        {
            try
            {
                _opcClient = new OpcClient(_opcuaConfig.CurrentValue.BaseUrl);
                _opcClient.Connect();

                _logger.LogInformation("Client Connected");
            }
            catch (Exception ex)
            {
                _logger.LogError("Client failed to connect. Message: {message}", ex.Message);
                throw;
            }
        }


        public void Publish(string tag, string payload)
        {
            _logger.LogInformation("OpcuaClient Publishing tag: {tag} with payload: {payload}", tag, payload);
        }

    }
}
