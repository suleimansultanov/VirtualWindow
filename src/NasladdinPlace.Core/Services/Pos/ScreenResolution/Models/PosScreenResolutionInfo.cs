using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;

namespace NasladdinPlace.Core.Services.Pos.ScreenResolution.Models
{
    public class PosScreenResolutionInfo
    {       
        public PosShortInfo PosInfo { get; }
        public UpdatableScreenResolution ScreenResolution { get; }

        public PosScreenResolutionInfo(PosShortInfo posInfo, UpdatableScreenResolution screenResolution)
        {
            if (posInfo == null)
                throw new ArgumentNullException(nameof(posInfo));
            if(screenResolution == null)
                throw new ArgumentNullException(nameof(screenResolution));

            PosInfo = posInfo;
            ScreenResolution = screenResolution;
        }
    }
}
