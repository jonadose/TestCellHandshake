
namespace TestCellHandshake.OpcuaService.Service
{
    public interface IOpcuaService
    {
        public void InitializeClient();
        public void Publish(string tag, string payload);
    }
}
