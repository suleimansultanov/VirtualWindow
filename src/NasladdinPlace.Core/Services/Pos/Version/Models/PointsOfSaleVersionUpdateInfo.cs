using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Core.Services.Pos.Version.Models
{
    public class PointsOfSaleVersionUpdateInfo
    {
        public ICollection<PosVersionInfo> PointsOfSaleVersionInfo { get; }
        public string RequiredMinVersion { get; }

        public PointsOfSaleVersionUpdateInfo(string requiredMinVersion, IEnumerable<PosVersionInfo> pointsOfSaleVersionInfo)
        {
            if (string.IsNullOrWhiteSpace(requiredMinVersion))
                throw new ArgumentNullException(nameof(requiredMinVersion));
            if (pointsOfSaleVersionInfo == null)
                throw new ArgumentNullException(nameof(pointsOfSaleVersionInfo));

            RequiredMinVersion = requiredMinVersion;
            PointsOfSaleVersionInfo = pointsOfSaleVersionInfo.ToImmutableList();
        }

        public bool IsUpdateRequired => PointsOfSaleVersionInfo.Any();
    }
}