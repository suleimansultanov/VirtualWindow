using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.UI.ViewModels.PointsOfSale
{
    public class SelectableOperationModeViewModel
    {
        public int OperationModeAsInt { get; set; }
        public bool IsSelected { get; set; }

        public PosMode GetOperationMode()
        {
            return Enum.Parse<PosMode>(OperationModeAsInt.ToString());
        }
    }
}