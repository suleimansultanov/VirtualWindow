using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Utils;
using NasladdinPlace.UI.Managers.Reference.Interfaces;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class PosBasicInfoViewModel : BaseViewModel, IComboboxViewModel
    {
        public int PosId { get; set; }

        public string Name { get; set; }

        public string GetText()
        {
            return Name;
        }

        public object GetValue()
        {
            return PosId;
        }

        public override Type EntityType()
        {
            return typeof(Pos);
        }

        protected bool Equals(PosBasicInfoViewModel other)
        {
            return PosId == other.PosId && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PosBasicInfoViewModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PosId * HashHelper.Salt) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}
