using TestCellHandshake.ApplicationLogic.Channels.Requests;

namespace TestCellHandshake.ApplicationLogic.Services
{
    public class BusinessLogicHandshakeService : IBusinessLogicHandshakeService
    {
        public Task HandleHandshakeRequest(HandshakeRequest? handshakeRequest)
        {
            throw new NotImplementedException();
        }


        public Task HandleCompleteHandshakeRequest(CompleteHandshakeRequest? completeHandshakeRequest)
        {
            // THis should do what // await PublishDeviceNewDataRec(false); did in logic handling service
            throw new NotImplementedException();
        }

    }
}
