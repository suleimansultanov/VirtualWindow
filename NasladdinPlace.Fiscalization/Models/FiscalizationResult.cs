using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class FiscalizationResult
    {
        public string Id { get; }
        public int ErrorCode { get; }

        public static FiscalizationResult Fiscalized(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return new FiscalizationResult(id, 0);
        }

        private FiscalizationResult(string id, int errorCode)
        {
            Id = id;
            ErrorCode = errorCode;
        }
    }
}
