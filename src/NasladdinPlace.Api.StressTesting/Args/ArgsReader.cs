using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Api.StressTesting.Args.Constrants;
using NasladdinPlace.Api.StressTesting.Args.Contracts;
using NasladdinPlace.Api.StressTesting.Models;

namespace NasladdinPlace.Api.StressTesting.Args
{
    public class ArgsReader : IArgsReader
    {   
        public bool TryReadConfig(string[] args, out StressTestingConfig config)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var argValueByNameDictionary = CreateArgValueByNameDictionary(args);

            return TryCreateConfig(argValueByNameDictionary, out config);
        }

        private static Dictionary<string, string> CreateArgValueByNameDictionary(IEnumerable<string> args)
        {
            var argValueByNameDictionary = new Dictionary<string, string>();
            
            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg)) continue;

                var lowerCasedArg = arg.ToLower();
                var argName = CommandLineArguments.All.First(cla => lowerCasedArg.Contains(cla.ToLower()));
                
                if (string.IsNullOrWhiteSpace(argName)) continue;

                var argValue = arg.Replace($"--{argName}=", string.Empty);
                
                if (string.IsNullOrWhiteSpace(argValue)) continue;

                argValueByNameDictionary[argName] = argValue;
            }

            return argValueByNameDictionary;
        }

        private static bool TryCreateConfig(IReadOnlyDictionary<string, string> argValueByNameDictionary,
            out StressTestingConfig config)
        {
            config = null;

            var userName = argValueByNameDictionary[CommandLineArguments.UserName];
            var password = argValueByNameDictionary[CommandLineArguments.UserSecret];
            var apiBaseUrl = argValueByNameDictionary[CommandLineArguments.BaseApiUrl];
            var concurrentRequestsNumber = argValueByNameDictionary[CommandLineArguments.ConcurrentRequestsNumber];

            try
            {
                var userCredentials = new UserCredentials(userName, password);
                var stressTestingConfig = new StressTestingConfig(apiBaseUrl, new List<UserCredentials>
                {
                    userCredentials
                });
                if (int.TryParse(concurrentRequestsNumber, out var concurrentRequestsNumberAsInt))
                {
                    stressTestingConfig.ConcurrentRequestsNumber = concurrentRequestsNumberAsInt;
                }

                config = stressTestingConfig;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}