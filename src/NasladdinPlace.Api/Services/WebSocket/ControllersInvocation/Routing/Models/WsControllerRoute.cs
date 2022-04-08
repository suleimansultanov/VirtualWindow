using System;

namespace NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models
{
    public class WsControllerRoute
    {
        private string Controller { get; }
        private string Action { get; }

        public WsControllerRoute()
        {
            Controller = string.Empty;
            Action = string.Empty;
        }
        
        public WsControllerRoute(string controller, string action)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (action == null)
                throw new ArgumentException(nameof(action));

            Controller = controller;
            Action = action;
            
        }
        
        public string AdjustedController => Controller.ToLower().Replace("controller", string.Empty);

        public string AdjustedAction => Action.ToLower();
    }
}