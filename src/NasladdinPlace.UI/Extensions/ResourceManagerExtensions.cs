using System;
using System.Resources;

namespace NasladdinPlace.UI.Extensions
{
    public static class ResourceManagerExtensions
    {
        public static bool TryGetResourceValue(this ResourceManager resourceManager, string resourceName, out string resourceValue)
        {
            if(string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(resourceName);

            resourceValue = string.Empty;

            try
            {
                resourceValue = resourceManager.GetString(resourceName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
