using System;
using System.Runtime.Serialization;

namespace NasladdinPlace.Core.Services.Shared.Models
{
    [Serializable]
    public class NoException : Exception
    {
        public NoException()
        {
            // intentionally left empty
        }

        protected NoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // intentionally left empty
        }
    }
}