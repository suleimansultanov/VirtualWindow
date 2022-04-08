using System;

namespace NasladdinPlace.Core.Models
{
    public sealed class PosQrCode : IEquatable<PosQrCode>
    {
        public int PosId { get; }
        public string QrCode { get; }
        public Guid CommandId { get; }

        public PosQrCode(int posId, string qrCode, Guid commandId)
        {
            PosId = posId;
            QrCode = qrCode;
            CommandId = commandId;
        }

        public bool Equals(PosQrCode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PosId == other.PosId && string.Equals(QrCode, other.QrCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PosQrCode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PosId * 397) ^ (QrCode != null ? QrCode.GetHashCode() : 0);
            }
        }
    }
}