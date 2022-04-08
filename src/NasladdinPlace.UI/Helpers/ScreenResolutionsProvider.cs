using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.UI.Helpers
{
    public static class ScreenResolutionsProvider
    {
        private static readonly Dictionary<ScreenResolutionType, ScreenResolution> PosScreenResolutions =
            new Dictionary<ScreenResolutionType, ScreenResolution>
            {
                {ScreenResolutionType.R1366X768, new ScreenResolution(1366, 768)},
                {ScreenResolutionType.R1680X1050, new ScreenResolution(1680, 1050)},
                {ScreenResolutionType.R1280X800, new ScreenResolution(1280, 800)},
                {ScreenResolutionType.R976X610, new ScreenResolution(976, 610)},
                {ScreenResolutionType.R1344X840, new ScreenResolution(1344, 840)}
            };

        public static bool TryProvideResolutionForType(ScreenResolutionType resolutionType, out ScreenResolution screenResolution)
        {
            return PosScreenResolutions.TryGetValue(resolutionType, out screenResolution);
        }

        public static bool TryProvideTypeForResolution(ScreenResolution screenResolution, out ScreenResolutionType resolutionType)
        {
            resolutionType = ScreenResolutionType.R1366X768;

            if (!PosScreenResolutions.ContainsValue(screenResolution))
                return false;

            resolutionType = PosScreenResolutions.FirstOrDefault(x => x.Value == screenResolution).Key;

            return true;
        }

        public static IReadOnlyCollection<ScreenResolution> GetValues()
        {
            return PosScreenResolutions.Values.ToImmutableList();
        }
    }
}