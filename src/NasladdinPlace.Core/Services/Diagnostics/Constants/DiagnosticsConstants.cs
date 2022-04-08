using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Constants
{
    public static class DiagnosticsConstants
    {
        public static readonly TimeSpan OpeningDoorTimeout = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan ClosingDoorsTimeout = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan InventoryTimeout = TimeSpan.FromSeconds(30);
    }
}