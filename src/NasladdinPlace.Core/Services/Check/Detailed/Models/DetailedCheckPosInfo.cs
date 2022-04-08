using System;

namespace NasladdinPlace.Core.Services.Check.Detailed.Models
{
    public class DetailedCheckPosInfo
    {
        public int PosId { get; }
        public string PosName { get; }
        public string PosAbbreviatedName { get; }

        public DetailedCheckPosInfo(int posId, string posName, string posAbbreviatedName)
        {
            if (string.IsNullOrEmpty(posName))
                throw new ArgumentNullException(nameof(posName));
            if (string.IsNullOrEmpty(posAbbreviatedName))
                throw new ArgumentNullException(nameof(posAbbreviatedName));

            PosId = posId;
            PosName = posName;
            PosAbbreviatedName = posAbbreviatedName;
        }

        public DetailedCheckPosInfo(Core.Models.Pos pos) 
            : this(pos.Id, pos.Name, pos.AbbreviatedName)
        {
        }
    }
}