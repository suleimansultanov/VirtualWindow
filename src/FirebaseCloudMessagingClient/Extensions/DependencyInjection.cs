using System;
using System.Net.Http;
using FirebaseCloudMessagingClient.Rest.Api;
using FirebaseCloudMessagingClient.Services;
using FirebaseCloudMessagingClient.Utils;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.PushNotifications;
using Refit;

namespace FirebaseCloudMessagingClient.Extensions
{
    public static class DependencyInjection
    {
	    public static void AddFirebaseCloudMessagingClient( this IServiceCollection services, string apiKey,
		    IConfigurationReader configurationReader )
	    {
		    var firebaseCloudMessagingApiUrl = configurationReader.GetFirebaseCloudMessagingApiUrl();
		    var firebaseTokenApiUrl = configurationReader.GetFirebaseTokenApiUrl();

		    services.AddTransient( sp =>
			    RestService.For<IFirebaseCloudMessagingApi>( firebaseCloudMessagingApiUrl )
		    );

		    services.AddTransient( sp =>
		    {
			    var httpClient = new HttpClient( new UriQueryUnescapingHandler() )
			    {
				    BaseAddress = new Uri( firebaseTokenApiUrl )
			    };

			    return RestService.For<IFirebaseTokenApi>( httpClient );
		    } );

		    services.AddTransient<IPushNotificationsService>( sp =>
			    new FirebasePushNotificationsService(
				    apiKey,
				    sp.GetRequiredService<IFirebaseCloudMessagingApi>(),
				    sp.GetRequiredService<IFirebaseTokenApi>(),
				    sp.GetRequiredService<INotificationsLogger>()
			    )
		    );
	    }
    }
}