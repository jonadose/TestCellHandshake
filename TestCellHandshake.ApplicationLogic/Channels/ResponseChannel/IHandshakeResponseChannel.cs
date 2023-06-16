using System.Threading.Channels;
using TestCellHandshake.ApplicationLogic.Channels.Commands;

namespace TestCellHandshake.ApplicationLogic.Channels.ResponseChannel
{
    public interface IHandshakeResponseChannel
    {
        ChannelReader<BaseMainCommand> Reader { get; }

        Task<bool> AddCommandAsync(BaseMainCommand command, CancellationToken cancellationToken = default);
        IAsyncEnumerable<BaseMainCommand> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}
