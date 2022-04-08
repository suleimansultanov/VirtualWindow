using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace NasladdinPlace.Api.Services.ViewRender
{
    public class ViewRender
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewRender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual string Render<TModel>(string name, TModel model)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var viewEngine = scope.ServiceProvider.GetRequiredService<IRazorViewEngine>();
                var tempDataProvider = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();
                
                var actionContext = GetActionContext(scope);

                var viewEngineResult = viewEngine.FindView(actionContext, name, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
                }

                var view = viewEngineResult.View;

                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        new ViewDataDictionary<TModel>(
                            metadataProvider: new EmptyModelMetadataProvider(),
                            modelState: new ModelStateDictionary())
                        {
                            Model = model
                        },
                        new TempDataDictionary(
                            actionContext.HttpContext,
                            tempDataProvider),
                        output,
                        new HtmlHelperOptions());

                    view.RenderAsync(viewContext).GetAwaiter().GetResult();

                    return output.ToString();
                }
            }
        }

        private ActionContext GetActionContext(IServiceScope serviceScope)
        {
            var httpContext = new DefaultHttpContext {RequestServices = serviceScope.ServiceProvider};
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}