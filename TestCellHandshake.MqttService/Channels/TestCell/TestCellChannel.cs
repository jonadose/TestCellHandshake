using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TestCellHandshake.MqttService.Commands;

namespace TestCellHandshake.MqttService.Channels.TestCell
{
    public class TestCellChannel : ITestCellChannel
    {
        public ChannelReader<BaseMainCommand> Reader => _channel.Reader;
        private readonly Channel<BaseMainCommand> _channel;

        public TestCellChannel()
        {
            var options = new BoundedChannelOptions(50)
            {
                SingleWriter = false,
                SingleReader = true
            };

            _channel = Channel.CreateBounded<BaseMainCommand>(options);
        }


        public async Task<bool> AddCommandAsync(BaseMainCommand command, CancellationToken cancellationToken = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(command))
                {
                    return true;
                }
            }
            return false;
        }

        public IAsyncEnumerable<BaseMainCommand> ReadAllAsync(CancellationToken cancellationToken = default) =>
            _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
