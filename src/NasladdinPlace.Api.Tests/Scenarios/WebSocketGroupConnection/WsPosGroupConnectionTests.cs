using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Tests.Scenarios.WebSocketGroupConnection.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Status;
using NasladdinPlace.TestUtils;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.Logging;
using NasladdinPlace.TestUtils.Seeding.Data;

namespace NasladdinPlace.Api.Tests.Scenarios.WebSocketGroupConnection
{
    public class WsPosGroupConnectionTests : TestsBase
    {
        private const int FirstPosId = 1;
        private const int SecondPosId = 2;
        private readonly int[] TestPosIds = { 1, 2, 3 };
        private Dictionary<int, AutoResetEvent> _eventsDictionary;
        private AutoResetEvent _queueIsEmptyForFirstPosWaitHandle;
        private AutoResetEvent _queueIsEmptyForSecondPosWaitHandle;
        private AutoResetEvent _queueIsEmptyForThirdPosWaitHandle;

        private const string AddToGroupEvent = "addToGroup";
        private const string AddToGroupActivity = "PlantHub";

        private const string PosGroup = "Plant_";
        
        private IPosConnectionStatusDetector _posConnectionStatusDetector;
        private NasladdinWebSocketDuplexEventMessageHandler _nasladdinWebSocketDuplexEventMessageHandler;
        private IServiceProvider _serviceProvider;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var serviceProviderFactory = new WebSocketServiceProviderFactory();
            _serviceProvider = serviceProviderFactory.CreateServiceProvider();

            _queueIsEmptyForFirstPosWaitHandle = new AutoResetEvent(false);
            _queueIsEmptyForSecondPosWaitHandle = new AutoResetEvent(false);
            _queueIsEmptyForThirdPosWaitHandle = new AutoResetEvent(false);

            _eventsDictionary = new Dictionary<int, AutoResetEvent>
            {
                {1, _queueIsEmptyForFirstPosWaitHandle },
                {2, _queueIsEmptyForSecondPosWaitHandle },
                {3, _queueIsEmptyForThirdPosWaitHandle }
            };

            var mockPosRealTimeInfo = _serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();

            foreach (var posId in TestPosIds)
            {
                var processor = CreateWsCommandsQueueProcessorAndSubscribeOnEvent(_eventsDictionary[posId]);

                mockPosRealTimeInfo.Setup(p => p.GetOrAddById(posId))
                    .Returns(new PosRealTimeInfo(posId)
                    {
                        CommandsQueueProcessor = processor
                    });
            }
            
            _posConnectionStatusDetector = _serviceProvider.GetRequiredService<IPosConnectionStatusDetector>();
            _nasladdinWebSocketDuplexEventMessageHandler =
                _serviceProvider.GetRequiredService<NasladdinWebSocketDuplexEventMessageHandler>();
        }

        [TestCase(FirstPosId)]
        [TestCase(SecondPosId)]
        public void
            ConnectAndDisconnectFromPosGroup_WebSocketAndPosGroupConnectionMessageAreGiven_ShouldConnectAndDisconnectFromPosGroup(
                int posId)
        {
            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var webSocket = CreateWebSocket();
            _nasladdinWebSocketDuplexEventMessageHandler.OnConnected(webSocket, IPAddress.Any).Wait();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var groupInfo = new GroupInfo
            {
                Group = $"Plant_{posId}"
            };

            var joinGroupEventMessage = new EventMessage
            {
                Event = AddToGroupEvent,
                Activity = AddToGroupActivity,
                Body = groupInfo
            };

            var joinGroupEventMessageAsJson = JsonConvert.SerializeObject(joinGroupEventMessage);

            _nasladdinWebSocketDuplexEventMessageHandler.ReceiveAsync(webSocket, null, joinGroupEventMessageAsJson)
                .Wait();

            _eventsDictionary[posId].WaitOne();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Connected);

            _nasladdinWebSocketDuplexEventMessageHandler.OnDisconnected(webSocket).Wait();
            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);
        }

        [TestCase(FirstPosId)]
        [TestCase(SecondPosId)]
        public void
            ConnectToPosGroupAndSendMessage_WebSocketAndPosGroupConnectionMessageAreGiven_ShouldConnectToPosGroupAndStayConnectedAfterSendingMessage(
                int posId)
        {
            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var webSocket = CreateWebSocket();
            _nasladdinWebSocketDuplexEventMessageHandler.OnConnected(webSocket, IPAddress.Any).Wait();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var groupInfo = new GroupInfo
            {
                Group = $"{PosGroup}{posId}"
            };

            var joinGroupEventMessage = new EventMessage
            {
                Event = AddToGroupEvent,
                Activity = AddToGroupActivity,
                Body = groupInfo
            };

            var joinGroupEventMessageAsJson = JsonConvert.SerializeObject(joinGroupEventMessage);

            _nasladdinWebSocketDuplexEventMessageHandler.ReceiveAsync(webSocket, null, joinGroupEventMessageAsJson)
                .Wait();

            _eventsDictionary[posId].WaitOne();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Connected);

            _nasladdinWebSocketDuplexEventMessageHandler.SendMessageToGroupAsync(
                $"{PosGroup}{posId}", "Test message"
            ).Wait();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Connected);
        }
        
        [TestCase(FirstPosId)]
        [TestCase(SecondPosId)]
        public void
            ConnectToPosGroupAndSendMessage_WebSocketWhichAbortsOnConnectionAndPosGroupConnectionMessageAreGiven_ShouldConnectToPosGroupAndNotStayConnectedAfterSendingMessage(
                int posId)
        {
            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var webSocket = CreateWebSocket();
            _nasladdinWebSocketDuplexEventMessageHandler.OnConnected(webSocket, IPAddress.Any).Wait();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);

            var groupInfo = new GroupInfo
            {
                Group = $"{PosGroup}{posId}"
            };

            var joinGroupEventMessage = new EventMessage
            {
                Event = AddToGroupEvent,
                Activity = AddToGroupActivity,
                Body = groupInfo
            };

            var joinGroupEventMessageAsJson = JsonConvert.SerializeObject(joinGroupEventMessage);

            _nasladdinWebSocketDuplexEventMessageHandler.ReceiveAsync(
                webSocket, null, joinGroupEventMessageAsJson
            ).Wait();

            _eventsDictionary[posId].WaitOne();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Connected);
            
            webSocket.Abort();

            _nasladdinWebSocketDuplexEventMessageHandler.SendMessageToGroupAsync(
                $"{PosGroup}{posId}", "Test message"
            ).Wait();

            AssertExpectedPosConnectionStatus(posId, PosConnectionStatus.Disconnected);
        }

        private void AssertExpectedPosConnectionStatus(int posId, PosConnectionStatus posConnectionStatus)
        {
            var status = _posConnectionStatusDetector.Detect(posId);
            status.ConnectionStatus.Should().Be(posConnectionStatus);
        }

        private static WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(new MemoryStream(), true, null, TimeSpan.FromMinutes(1));
        }

        private WsCommandsQueueProcessor CreateWsCommandsQueueProcessorAndSubscribeOnEvent(EventWaitHandle queueIsEmptyWaitHandler)
        {
            var processor = new WsCommandsQueueProcessor(
                10,
                TimeSpan.FromMilliseconds(3000),
                _serviceProvider.GetRequiredService<ILogger>());

            processor.OnQueueProcessed += (sender, eventArgs) =>
            {
                queueIsEmptyWaitHandler.Set();
            };

            return processor;
        }
    }
}