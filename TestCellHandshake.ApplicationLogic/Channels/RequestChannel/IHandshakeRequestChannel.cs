using System.Threading.Channels;
using TestCellHandshake.ApplicationLogic.Channels.Requests;

namespace TestCellHandshake.ApplicationLogic.Channels.RequestChannel
{
    public interface IHandshakeRequestChannel
    {
        ChannelReader<BaseRequest> Reader { get; }

        Task<bool> AddCommandAsync(BaseRequest request, CancellationToken cancellationToken = default);
        IAsyncEnumerable<BaseRequest> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}
