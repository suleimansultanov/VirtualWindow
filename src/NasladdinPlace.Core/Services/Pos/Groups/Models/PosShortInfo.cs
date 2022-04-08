using System;

namespace NasladdinPlace.Core.Services.Pos.Groups.Models
{
    public class PosShortInfo
    {
        public static readonly PosShortInfo Empty = new PosShortInfo(0, string.Empty);
        
        public static PosShortInfo FromPos(Core.Models.Pos pos)
        {
            if (pos == null)
                throw new ArgumentNullException(nameof(pos));
            
            return new PosShortInfo(pos.Id, pos.AbbreviatedName);
        }
        
        public int Id { get; }
        public string Name { get; }

        public PosShortInfo(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}