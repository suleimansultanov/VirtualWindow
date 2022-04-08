using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocument(this IServiceCollection services, string title)
        {
            services.AddSwaggerDocument(config =>
            {
                var securityName = "Bearer";

                config.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor(securityName));

                config.AddSecurity(securityName, Enumerable.Empty<string>(),
                    new SwaggerSecurityScheme()
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = nameof(Authorization),
                        In = SwaggerSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: \nBearer {your token}"
                    }
                );

                config.PostProcess = document =>
                {
                    document.Info.Title = title;
                };
            });
        }

        public static void UseSwaggerUi( this IApplicationBuilder app, IConfigurationReader configurationReader )
        {
	        if ( configurationReader == null )
		        throw new ArgumentNullException( nameof( configurationReader ) );

	        var swaggerPath = configurationReader.GetSwaggerPagePath();

	        app.UseSwagger();
	        app.UseSwaggerUi3( config => { config.Path = swaggerPath; } );
        }
    }
}
