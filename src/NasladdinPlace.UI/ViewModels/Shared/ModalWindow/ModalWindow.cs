namespace NasladdinPlace.UI.ViewModels.Shared.ModalWindow
{
    public class ModalWindow
    {
        public string Id { get; set; }
        public ModalWindowSize Size { get; set; }

        public string ModalSizeCssClass
        {
            get
            {
                switch (this.Size)
                {
                    case ModalWindowSize.Small:
                        return "modal-sm";
                    case ModalWindowSize.Large:
                        return "modal-lg";
                    default:
                        return "";
                }
            }
        }
    }
}
