using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.UI.Dtos.LabeledGood
{
    public class LabelsDto
    {
        public ICollection<string> Labels { get; set; }

        public LabelsDto()
        {
            Labels = new Collection<string>();
        }
    }
}