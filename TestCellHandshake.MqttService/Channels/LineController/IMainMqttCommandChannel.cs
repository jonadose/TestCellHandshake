using System.Threading.Channels;
using TestCellHandshake.MqttService.Commands;

namespace TestCellHandshake.MqttService.Channels.LineController
{
    public interface IMainMqttCommandChannel
    {
        ChannelReader<BaseMainCommand> Reader { get; }

        Task<bool> AddCommandAsync(BaseMainCommand command, CancellationToken cancellationToken = default);
        IAsyncEnumerable<BaseMainCommand> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}
