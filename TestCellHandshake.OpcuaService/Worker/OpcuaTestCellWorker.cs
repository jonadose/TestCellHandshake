using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace TestCellHandshake.OpcuaService.Worker
{
    public class OpcuaTestCellWorker : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}