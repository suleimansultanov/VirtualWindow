using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.Pos.WebSocket.Factory
{
    public class WsCommandsQueueProcessorFactory : IWsCommandsQueueProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WsCommandsQueueProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public  IWsCommandsQueueProcessor Create()
        {
            var configurationReader = _serviceProvider.GetRequiredService<IConfigurationReader>();
            var distinctCommandsIdCountLimit = configurationReader.GetDistinctCommandsIdCountLimit();
            var commandsExecutionDelay = configurationReader.GetCommandWaitingTimeoutInMilliseconds();

            var logger = _serviceProvider.GetRequiredService<ILogger>();

            return new WsCommandsQueueProcessor(distinctCommandsIdCountLimit, TimeSpan.FromMilliseconds(commandsExecutionDelay), logger);
        }
    }
}
