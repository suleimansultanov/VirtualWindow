using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.WebSocket.Managers.ConnectionManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using Newtonsoft.Json;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers
{
    public abstract class DuplexEventMessageHandler : WebSocketHandler
    {
        private readonly IStringUnescaper _stringUnescaper;

        protected DuplexEventMessageHandler(IWebSocketConnectionManager webSocketConnectionManager, 
            IStringUnescaper stringUnescaper) 
            : base(webSocketConnectionManager)
        {
            _stringUnescaper = stringUnescaper;
        }

        public async Task SendEventToAllAsync(EventMessage eventMessage)
        {
            await SendMessageToAllAsync(JsonConvert.SerializeObject(eventMessage));
        }

        public async Task SendEventToGroupAsync(string group, EventMessage eventMessage)
        {
            await SendMessageToGroupAsync(group, JsonConvert.SerializeObject(
                eventMessage,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public override async Task ReceiveAsync(System.Net.WebSockets.WebSocket socket, WebSocketReceiveResult result, string message)
        {
            try
            {
                var eventMessage = JsonConvert.DeserializeObject<EventMessage>(_stringUnescaper.Unescape(message));
                await HandleReceivedEventMessageAsync(socket, eventMessage);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(socket, ex.ToString());
            }
        }

        protected abstract Task HandleReceivedEventMessageAsync(System.Net.WebSockets.WebSocket webSocket, EventMessage eventMessage);
    }
}
