using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Tests.DataGenerators.Models
{
    public class OperationManagerSource
    {
        public Collection<PosOperation> PosOperations { get; set; }
        public PosOperationStatus ExpectedStatus { get; set; }
        public decimal ExpectedBonus { get; set; }
        public int ExpectedBonusesCount { get; set; }

        public OperationManagerSource()
        {
            PosOperations = new Collection<PosOperation>();
        }
    }
}