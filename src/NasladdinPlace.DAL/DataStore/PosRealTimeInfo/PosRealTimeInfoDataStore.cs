using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.Core.Services.Pos.WebSocket.Factory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NasladdinPlace.DAL.DataStore.PosRealTimeInfo
{
    public class PosRealTimeInfoDataStore : IPosRealTimeInfoDataStore
    {
        private static readonly ConcurrentDictionary<int, Core.Models.PosRealTimeInfo> PosRealTimeInfoByIdDictionary
            = new ConcurrentDictionary<int, Core.Models.PosRealTimeInfo>();

        private readonly IPosConnectionStatusDetector _posConnectionStatusDetector;
        private readonly IWsCommandsQueueProcessorFactory _wsCommandsQueueProcessorFactory;

        public PosRealTimeInfoDataStore(
            IPosConnectionStatusDetector posConnectionStatusDetector,
            IWsCommandsQueueProcessorFactory wsCommandsQueueProcessorFactory)
        {
            if (posConnectionStatusDetector == null)
                throw new ArgumentNullException(nameof(posConnectionStatusDetector));
            if (wsCommandsQueueProcessorFactory == null)
                throw new ArgumentNullException(nameof(wsCommandsQueueProcessorFactory));

            _posConnectionStatusDetector = posConnectionStatusDetector;
            _wsCommandsQueueProcessorFactory = wsCommandsQueueProcessorFactory;
        }

        public Core.Models.PosRealTimeInfo GetOrAddById(int id)
        {
            var posRealTimeInfo = PosRealTimeInfoByIdDictionary.GetOrAdd(
                id, (posId) =>
                new Core.Models.PosRealTimeInfo(posId)
                {
                    CommandsQueueProcessor = _wsCommandsQueueProcessorFactory.Create()
                });

            var posConnectionInfoDetect = _posConnectionStatusDetector.Detect(id);

            posRealTimeInfo.ConnectionStatus = posConnectionInfoDetect.ConnectionStatus;
            posRealTimeInfo.IpAddresses = posConnectionInfoDetect.IpAddresses;

            return posRealTimeInfo;
        }

        public IEnumerable<Core.Models.PosRealTimeInfo> GetAll()
        {
            return PosRealTimeInfoByIdDictionary.Values.ToImmutableList();
        }
    }
}