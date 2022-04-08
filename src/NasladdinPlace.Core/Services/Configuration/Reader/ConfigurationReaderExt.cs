using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
	public static class ConfigurationReaderExt
	{
		internal static T GetValueByKey<T>( this IConfigurationReader configurationReader, ConfigurationKeyIdentifier key )
		{
			if ( configurationReader == null )
				throw new ArgumentNullException( nameof( configurationReader ) );

			if ( !configurationReader.TryGetValueByKey<T>( key, out var value ) )
				throw new InvalidOperationException( $"Parameter '{key}' does not found in database configuration settings." );

			return value;
		}

		internal static IEnumerable<T> GetValuesByKey<T>( this IConfigurationReader configurationReader, ConfigurationKeyIdentifier key ) {
			if ( configurationReader == null )
				throw new ArgumentNullException( nameof( configurationReader ) );

			if ( !configurationReader.TryGetValuesByKey<T>( key, out var value ))
				throw new InvalidOperationException( $"Parameter '{key}' does not found in database configuration settings." );

			return value;
		}

		public static string CombineUrlParts( string partOne, string partTwo )
		{
			if ( string.IsNullOrEmpty( partOne ) )
				throw new ArgumentNullException( nameof( partOne ) );

			if ( string.IsNullOrEmpty( partTwo ) )
				throw new ArgumentNullException( nameof( partTwo ) );

			if ( !Uri.TryCreate( new Uri( partOne ), partTwo, out var result ) )
				throw new InvalidOperationException( $"Can't build Uri from parts: '{partOne}' & '{partTwo}'." );

			return result.OriginalString;
		}


		public static string GetWsUrl(string url)
		{
            const string httpPattern = "^http";
            Regex regex = new Regex(httpPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if(!regex.IsMatch(url))
                throw new InvalidOperationException($"Parameter '{nameof(url)}' would match with pattern '{httpPattern}'.");
            url = regex.Replace(url, "ws");
            return url;
		}
	}
}