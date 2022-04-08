using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Check.CommonModels
{
    public class CheckStatusInfo
    {
        public SimpleCheckStatus Status { get; }
        public DateTime DateModified { get; }

        public static readonly CheckStatusInfo Unmodified = new CheckStatusInfo(SimpleCheckStatus.Unmodified, DateTime.MinValue);

        public CheckStatusInfo(SimpleCheckStatus status, DateTime dateModified)
        {
            if (!Enum.IsDefined(typeof(SimpleCheckStatus), status))
                throw new ArgumentException(nameof(status));

            Status = status;
            DateModified = dateModified;
        }
    }
}
