using System;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckOriginInfo
    {
        public static readonly SimpleCheckOriginInfo Unknown = new SimpleCheckOriginInfo("Unknown"); 
        
        public string PosName { get; }

        public SimpleCheckOriginInfo(string posName)
        {
            if (string.IsNullOrWhiteSpace(posName))
                throw new ArgumentNullException(nameof(posName));
            
            PosName = posName;
        }
    }
}