using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.CommandDelivery;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Api.Dtos.PosAntennasOutputPower;
using NasladdinPlace.Api.Dtos.PosDisplay;
using NasladdinPlace.Api.Dtos.PosLogs;
using NasladdinPlace.Api.Dtos.QrCode;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Factories;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages.Mappers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers
{
    public class NasladdinWebSocketDuplexEventMessageHandler : DuplexEventMessageHandler, INasladdinWebSocketMessageSender
    {
        private const string EventShowContent = "handleCheck";
        private const string ActivityPlantDisplay = "PlantDisplay";
        public const string ActivityLabeledGoods = "LabeledGoods";
        public const string EventGetUntiedLabeledGoods = "GetUntiedLabeledGoods";

        private readonly IWsControllerInvoker _wsControllerInvoker;
        private readonly IEventMessageToWsMessageMapper _eventMessageToWsMessageMapper;
        private readonly IActivityMessageFactory _posActivityMessageFactory;

        public NasladdinWebSocketDuplexEventMessageHandler(
            IWebSocketConnectionManager webSocketConnectionManager, 
            IStringUnescaper stringUnescaper,
            IWsControllerInvoker wsControllerInvoker,
            IEventMessageToWsMessageMapper eventMessageToWsMessageMapper)
            : base(webSocketConnectionManager, stringUnescaper)
        {
            _wsControllerInvoker = wsControllerInvoker;
            _eventMessageToWsMessageMapper = eventMessageToWsMessageMapper;
            _posActivityMessageFactory = new PosActivityMessageFactory();
        }

        public override ILogger Logger
        {
            get => base.Logger;
            set
            {
                base.Logger = value;
                WebSocketConnectionManager.Logger = value;
            }
        }

        public Task RequestPosOperationInitiationAsync(int posId, PosMode posMode, PosDoorPosition doorPosition)
        {
            var posOperationInitiationParamsDto = new PosOperationInitiationParamsDto(doorPosition, posMode);
            return SendPosEventMessageAsync(posId, PosEvents.StartPosOperationInMode, posOperationInitiationParamsDto);
        }

        public Task RequestPosOperationContinuationAsync(int posId)
        {
            return SendPosEventMessageAsync(posId, PosEvents.ContinueOperation);
        }

        public Task RequestPosOperationCompletionAsync(int posId)
        {
            return SendPosEventMessageAsync(posId, PosEvents.CompleteOperation);
        }

        public Task RequestPosAccountingBalancesAsync(int posId)
        {
            return SendPosEventMessageAsync(posId, PosEvents.RequestAccountingBalances);
        }

        public Task RequestPosAntennasOutputPowerAsync(int posId)
        {
            return SendPosEventMessageAsync(posId, PosEvents.RequestAntennasOutputPower);
        }

        public Task RequestDoorsStateAsync(int posId)
        {
            return SendPosEventMessageAsync(posId, PosEvents.RequestDoorsState);
        }

        public Task ConfirmCommandDelivery(int posId, Guid commandId)
        {
            var confirmCommandDeliveryDto = new ConfirmCommandDeliveryDto
            {
                CommandId = commandId
            };
            return SendPosEventMessageAsync(posId, PosEvents.ConfirmCommandDelivery, confirmCommandDeliveryDto);
        }

        public Task RequestLogsAsync(int posId, PosLogType posLogType)
        {
            var request = new RequestPosLogDto(posLogType);
            return SendPosEventMessageAsync(posId, PosEvents.RequestLogs, request);
        }

        public Task SetPosAntennasOutputPowerAsync(int posId, PosAntennasOutputPower antennasOutputPower)
        {
            var posAntennasOutputPowerDto = new PosAntennasOutputPowerDto
            {
                PosId = posId,
                OutputPower = antennasOutputPower
            };
            return SendPosEventMessageAsync(posId, PosEvents.SetAntennasOutputPower, posAntennasOutputPowerDto);
        }

        public Task SendPosEventMessageAsync(int posId, string eventName, object body = null)
        {
            var eventMessage = _posActivityMessageFactory.MakeEventMessage(eventName, body);

            return SendEventToGroupAsync(Groups.Pos + posId, eventMessage);
        }

        public Task SendPosDisplayCheckAsync(int posId, CheckDto check)
        {
            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(check)
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posId, eventMessage);
        }

        public Task SendUntiedLabeledGoodsAsync(int posId, IEnumerable<LabeledGoodDto> labeledGoodDtos)
        {
            return SendEventToGroupAsync($"{Groups.PosUntiedLabels}{posId}", new EventMessage
            {
                Activity = ActivityLabeledGoods,
                Event = EventGetUntiedLabeledGoods,
                Body = labeledGoodDtos
            });
        }
        
        public Task NotifyPosQrCodeChangeAsync(PosQrCode posQrCode)
        {
            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(new QrCodeDto(posQrCode.QrCode, posQrCode.CommandId))
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posQrCode.PosId, eventMessage);
        }

        public Task NotifyHidePosQrCodeAsync(int posId, Guid commandId)
        {
            var command = new PosDisplayCommandDto(commandId);

            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(PosDisplayContentType.ActivePurchase, command)
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posId, eventMessage);
        }

        public Task SendTimerPresentationRequestAsync(int posId, Guid commandId)
        {
            var command = new PosDisplayCommandDto(commandId);

            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(PosDisplayContentType.Inventory, command)
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posId, eventMessage);
        }

        public Task SendPosDisconnectedPagePresentationRequestAsync(int posId)
        {
            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(contentType: PosDisplayContentType.Disconnect, content: null)
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posId, eventMessage);
        }

        public Task SendPosRefreshDisplayPageRequestAsync(int posId)
        {
            var eventMessage = new EventMessage
            {
                Activity = ActivityPlantDisplay,
                Event = EventShowContent,
                Body = new PosDisplayDto(contentType: PosDisplayContentType.Refresh, content: null)
            };
            return SendEventToGroupAsync(Groups.PosDisplay + posId, eventMessage);
        }

        protected override Task HandleReceivedEventMessageAsync(
            System.Net.WebSockets.WebSocket webSocket, EventMessage eventMessage)
        {
            var wsMessage = _eventMessageToWsMessageMapper.Transform(eventMessage);
            return _wsControllerInvoker.InvokeAsync(webSocket, wsMessage);
        } 
    }
}
