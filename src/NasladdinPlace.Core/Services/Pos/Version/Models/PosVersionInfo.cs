using System;
using NasladdinPlace.Core.Services.Pos.Groups.Models;

namespace NasladdinPlace.Core.Services.Pos.Version.Models
{
    public class PosVersionInfo
    {
        public PosShortInfo PosInfo { get; }
        public string CurrentVersion { get; }

        public PosVersionInfo(PosShortInfo posInfo, string currentVersion)
        {
            if (posInfo == null)
                throw new ArgumentNullException(nameof(posInfo));

            PosInfo = posInfo;
            CurrentVersion = currentVersion;
        }
    }
}