using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Tools;

namespace NasladdinPlace.Api.Tests.Utils.CheckOnline
{
	public static class TestCheckOnlineRequestProviderFactory
	{
		public static ICheckOnlineRequestProvider Create( IServiceProvider sp, bool useMock ) {
			if (sp == null)
				throw new ArgumentNullException(nameof(sp));

			if (useMock) 
				return new FakeCheckOnlineRequestProvider();

			return new CheckOnlineRequestProvider( sp.GetRequiredService<IHttpWebRequestProvider>() );
		}
	}
}