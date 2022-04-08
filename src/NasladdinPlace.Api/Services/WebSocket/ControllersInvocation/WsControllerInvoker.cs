using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers;
using NasladdinPlace.Api.Services.WebSocket.Managers.Messages;
using NasladdinPlace.Api.Services.WebSocket.Managers.Utils;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using NasladdinPlace.Core.Services.Pos.WebSocket.Models;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation
{
    public class WsControllerInvoker : IWsControllerInvoker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IObjectDeserializer _objectDeserializer;
        private readonly ConcurrentDictionary<string, Type> _controllerTypeByName;

        public WsControllerInvoker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _objectDeserializer = serviceProvider.GetService<IObjectDeserializer>();
            _controllerTypeByName = CreateControllerTypeByNameDictionary();
        }

        public async Task InvokeAsync(System.Net.WebSockets.WebSocket webSocket, WsMessage message)
        {
            var nasladdinDuplexEventMessageHandler = _serviceProvider.GetService<NasladdinWebSocketDuplexEventMessageHandler>();

            var route = message.Route;

            if (_controllerTypeByName.TryGetValue(route.AdjustedController, out var controllerType))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var controller = CreateInstance(controllerType, webSocket, scope);

                    var method = FindMethod(controllerType, route.AdjustedAction);

                    if (method == null)
                        return;

                    var methodParameter = CastToMethodParameterType(method, message.Body);

                    var methodParameters = new object[0];
                    if (methodParameter != null)
                    {
                        var (isValid, error) = ValidateModelState(methodParameter);
                        if (isValid)
                        {
                            methodParameters = new[] { methodParameter };
                        }
                        else
                        {
                            await nasladdinDuplexEventMessageHandler.SendMessageAsync(webSocket, error);
                        }
                    }

                    var invocationInfo = new ControllerInvocationInfo(method, controllerType.Name, controller, methodParameters);

                    await InvokeControllerMethodAsync(invocationInfo);
                }
            }
        }

        private async Task InvokeControllerMethodAsync(ControllerInvocationInfo invocationInfo)
        {
            var posId = TryGetPosIdFromParametersOrNull(invocationInfo.Parameters);
            if (posId.HasValue)
            {
                var unitOfWorkFactory = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
                Core.Models.Pos pos;
                PosRealTimeInfo posRealTimeInfo;
                using (var unitOfWork = unitOfWorkFactory.MakeUnitOfWork())
                {
                    pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId.Value);
                    posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId.Value);
                }

                if (pos == null)
                {
                    throw new ArgumentException(nameof(posId), $"Pos with id {posId.Value} not found");
                }

                posRealTimeInfo.LastReceivedWsMessage.Update(invocationInfo.ControllerName, invocationInfo.Method.Name, pos.PosActivityStatus);
                if (!posRealTimeInfo.LastReceivedWsMessage.IsDeactivatedPosReceivingWsMessages)
                {
                    await posRealTimeInfo.CommandsQueueProcessor.EnqueueAndProcessAsync(invocationInfo);
                }
            }
        }

        private int? TryGetPosIdFromParametersOrNull(object[] methodParameters)
        {
            var parameters = methodParameters[0];

            var posId = (int?)parameters.GetType().GetProperty(nameof(BasePosWsMessageDto.PosId))?.GetValue(parameters, null);

            //TODO: Ниже идет костыль, который нужен, чтобы вытащить Id витрины из параметра запроса
            //в том случае, если им является объект GroupInfo. В таком объекте Id витрины находится в поле 
            //Group и пишется в формате "Plant_1" или "PlantDispay_1". Нужно передедать все Dto, которые используются
            //для приема веб-сокетного соединения на то, чтобы они наследовались от BasePosWsMessageDto и Id витрины
            //хранилось в поле PosId. Но для этого также придется переделать код службы
            var groupInfo = parameters as GroupInfo;
            if (!posId.HasValue && groupInfo != null)
            {
                posId = Convert.ToInt32(groupInfo.Group.Split("_").Last());
                if (posId == 0)
                {
                    return null;
                }
            }

            return posId;
        }

        private static object CreateInstance(Type type, System.Net.WebSockets.WebSocket webSocket, IServiceScope scope)
        {
            var constructor = type.GetConstructors()[0];
            var constructorParameters = constructor.GetParameters();

            if (!constructorParameters.Any())
                return Activator.CreateInstance(type);

            var resolvedConstructorParameters = constructorParameters
                .Select(p => p.ParameterType == typeof(System.Net.WebSockets.WebSocket)
                    ? webSocket
                    : scope.ServiceProvider.GetRequiredService(p.ParameterType)
                )
            .ToArray();

            return Activator.CreateInstance(type, resolvedConstructorParameters);
        }

        private object CastToMethodParameterType(MethodBase method, object body)
        {
            if (body == null)
                return null;

            var firstParameter = method.GetParameters().FirstOrDefault();

            return firstParameter == null
                ? null
                : _objectDeserializer.Deserialize(body, firstParameter.ParameterType);
        }

        private static MethodInfo FindMethod(IReflect type, string methodName)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(m => m.Name.ToLower() == methodName);
        }

        private static (bool IsValid, string Error) ValidateModelState(object model)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results);

            if (isValid) return (true, string.Empty);

            var validationErrorsAsString = string.Join(", ", results.Select(s => s.ErrorMessage).ToArray());
            var errorMessage = "Model is not valid because " + validationErrorsAsString;
            return (false, errorMessage);
        }

        private static ConcurrentDictionary<string, Type> CreateControllerTypeByNameDictionary()
        {
            var controllerTypeByName = new ConcurrentDictionary<string, Type>();
            foreach (var type in Assembly.GetAssembly(typeof(WsController)).ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WsController))
                {
                    controllerTypeByName.TryAdd(type.Name.Replace("Controller", string.Empty).ToLower(), type);
                }
            }
            return controllerTypeByName;
        }
    }
}