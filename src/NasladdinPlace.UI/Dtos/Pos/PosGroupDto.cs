using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.UI.Dtos.Pos
{
    public class PosGroupDto<TElementDto>
    {
        public int Id { get; set; }
        public PosDto PosInfo { get; set; }
        public ICollection<TElementDto> Items { get; set; }

        public PosGroupDto()
        {
            Items = new Collection<TElementDto>();
        }
    }
}