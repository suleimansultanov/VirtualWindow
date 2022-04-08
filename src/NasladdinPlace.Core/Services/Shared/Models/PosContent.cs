using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Internal;

namespace NasladdinPlace.Core.Services.Shared.Models
{
    public sealed class PosContent
    {
        public int PosId { get; }
        public ICollection<string> Labels { get; }

        public PosContent(int posId, IEnumerable<string> labels)
        {
            if (labels == null)
                throw new ArgumentNullException(nameof(labels));

            PosId = posId;
            Labels = labels.ToImmutableSortedSet();
        }

        public bool Any() => Labels.Any();
    }
}