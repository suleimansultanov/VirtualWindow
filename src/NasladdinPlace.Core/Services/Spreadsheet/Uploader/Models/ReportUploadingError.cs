namespace NasladdinPlace.Core.Services.Spreadsheet.Uploader.Models
{
    public class ReportUploadingError
    {
        public int Code { get; }
        public string Message { get;}
        public string Type { get; }

        public ReportUploadingError(int code, string message, string type)
        {
            Code = code;
            Message = message;
            Type = type;
        }

    }
}
