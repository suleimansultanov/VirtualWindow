using System;

namespace NasladdinPlace.Core.Models
{
    public class PointsOfSaleToRole
    {
        public int RoleId { get; private set; }
        public int PosId { get; private set; }

        public PointsOfSaleToRole()
        {
            // required for EF
        }

        public PointsOfSaleToRole(int roleId, int posId)
        {
            if (roleId < 0)
                throw new ArgumentOutOfRangeException(nameof(roleId), roleId, "RoleId must be grater than zero.");
            if (posId < 0)
                throw new ArgumentOutOfRangeException(nameof(posId), posId, "PosId must be grater than zero.");

            RoleId = roleId;
            PosId = posId;
        }
    }
}
