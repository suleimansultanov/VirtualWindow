using System;
using System.Reflection;

namespace NasladdinPlace.Core.Services.Pos.WebSocket.Models
{
    public class ControllerInvocationInfo 
    {
        public MethodInfo Method { get; }

        public string ControllerName { get; }

        public object Controller { get; }

        public object[] Parameters { get; }

        public ControllerInvocationInfo(MethodInfo method, string controllerName, object controller, object[] parameters)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            if(controllerName == null)
                throw new ArgumentNullException(nameof(controllerName));
            if(controller == null)
                throw new ArgumentNullException(nameof(controller));
            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Method = method;
            ControllerName = controllerName;
            Controller = controller;
            Parameters = parameters;
        }
    }
}