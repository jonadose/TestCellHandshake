
using TestCellHandshake.ApplicationLogic.Channels.Requests;

namespace TestCellHandshake.ApplicationLogic.Services
{
    public interface IBusinessLogicHandshakeService
    {
        Task HandleHandshakeRequest(HandshakeRequest? handshakeRequest);
        Task HandleCompleteHandshakeRequest(CompleteHandshakeRequest? completeHandshakeRequest);
    }
}
