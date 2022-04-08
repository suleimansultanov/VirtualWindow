using AutoMapper;
using NasladdinPlace.Api.Dtos.Log;
using NasladdinPlace.Api.Services.WebSocket.Managers.Constants;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Logging.Models;
using NasladdinPlace.Logging.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Logs.Writers
{
    public class LogWriterViaWebSocket : ILogWriter
    {
        private readonly WebSocketHandler _webSocketHandler;

        public LogWriterViaWebSocket(WebSocketHandler webSocketHandler)
        {
            _webSocketHandler = webSocketHandler;
        }

        public Task Write(Log log)
        {
            var dto = Mapper.Map<LogDto>(log);
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(dto, Formatting.Indented, settings);
            return _webSocketHandler.SendMessageToGroupAsync(Groups.Logs, json);
        }
    }
}
