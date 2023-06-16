using System.Threading.Channels;
using TestCellHandshake.ApplicationLogic.Channels.Requests;

namespace TestCellHandshake.ApplicationLogic.Channels.RequestChannel
{
    public class HandshakeRequestChannel : IHandshakeRequestChannel
    {
        public ChannelReader<BaseRequest> Reader => _channel.Reader;
        private readonly Channel<BaseRequest> _channel;

        public HandshakeRequestChannel()
        {
            var options = new BoundedChannelOptions(50)
            {
                SingleWriter = false,
                SingleReader = true
            };

            _channel = Channel.CreateBounded<BaseRequest>(options);
        }


        public async Task<bool> AddCommandAsync(BaseRequest request, CancellationToken cancellationToken = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (_channel.Writer.TryWrite(request))
                {
                    return true;
                }
            }

            return false;
        }


        public IAsyncEnumerable<BaseRequest> ReadAllAsync(CancellationToken cancellationToken = default) =>
            _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
