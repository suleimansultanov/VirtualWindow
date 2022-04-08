namespace NasladdinPlace.UI.ViewModels.Results3Ds
{
    public class FailureResult3DsViewModel
    {
        public string Error { get; set; }

        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }
}