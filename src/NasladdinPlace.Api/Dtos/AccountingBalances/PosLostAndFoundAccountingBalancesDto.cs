using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Dtos.AccountingBalances
{
    public class PosLostAndFoundAccountingBalancesDto : BasePosWsMessageDto
    {
        public ICollection<string> LostLabels { get; set; }
        public ICollection<string> FoundLabels { get; set; }

        public PosLostAndFoundAccountingBalancesDto()
        {
            LostLabels = new Collection<string>();
            FoundLabels = new Collection<string>();
        }
        
        public ICollection<string> All => LostLabels.Union(FoundLabels).ToImmutableList();
    }
}