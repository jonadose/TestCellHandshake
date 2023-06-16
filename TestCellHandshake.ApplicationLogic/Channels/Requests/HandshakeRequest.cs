
namespace TestCellHandshake.ApplicationLogic.Channels.Requests
{
    public class HandshakeRequest : BaseRequest
    {
        public required string PowerUnitId { get; init; }
    }
}
