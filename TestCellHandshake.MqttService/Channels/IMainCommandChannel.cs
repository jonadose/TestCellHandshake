﻿using System.Threading.Channels;
using TestCellHandshake.ApplicationLogic.Channels.Commands;

namespace TestCellHandshake.MqttService.Channels
{
    public interface IMainCommandChannel
    {
        ChannelReader<BaseMainCommand> Reader { get; }

        Task<bool> AddCommandAsync(BaseMainCommand command, CancellationToken cancellationToken = default);
        IAsyncEnumerable<BaseMainCommand> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}
