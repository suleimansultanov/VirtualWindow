using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Users.Test;

namespace NasladdinPlace.Api.Services.TestUser.Extensions
{
    public static class TestUserInfoProviderExtensions
    {
	    public static void AddTestUserInfoProvider( this IServiceCollection services,
		    IConfigurationReader configurationReader )
	    {
		    if ( configurationReader == null )
			    throw new ArgumentNullException( nameof( configurationReader ) );

		    var userName = configurationReader.GetTestUserName();
		    var isTestUserPaymentCarVerificationRequired =
			    configurationReader.GetIsTestUserPaymentCardVerificationRequired();

		    services.AddSingleton<ITestUserInfoProvider>( sp =>
		    {
			    var unitOfWorkFactory = sp.GetRequiredService<IUnitOfWorkFactory>();
			    return new TestUserInfoProvider( unitOfWorkFactory, userName,
				    isTestUserPaymentCarVerificationRequired );
		    } );
	    }
    }
}