namespace NasladdinPlace.UI.ViewModels.Shared.ModalWindow

{
    public class ModalWindowFooter
    {
        public string SubmitButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string SubmitButtonId { get; set; }
        public string CancelButtonId { get; set; }
        public bool OnlyCancelButton { get; set; }
        public string SubmitButtonClass { get; set; }
        public string ButtonAttribute { get; set; }

        public ModalWindowFooter()
        {
            SubmitButtonId = "btn-submit";
            CancelButtonId = "btn-cancel";
        }
    }
}
