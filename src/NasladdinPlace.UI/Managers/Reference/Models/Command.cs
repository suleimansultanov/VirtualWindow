using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.Managers.Reference.Models
{
    public class Command
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public CommandType Type { get; set; }
        public string Binding { get; set; }
        public string ClassStyle { get; set; }
        public string ConfirmText { get; set; }
    }
}
