using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class ReportUploadingInfo : Entity
    {
        public ReportType Type { get; private set; }
        public string Url { get; private set; }
        public string Description { get; private set; }
        public string Sheet { get; private set; }
        public int BatchSize { get; private set; }

        public ReportUploadingInfo(string url, ReportType type, string description, string sheet, int batchSize)
        {
            Url = url;
            Type = type;
            Description = description;
            Sheet = sheet;
            BatchSize = batchSize;
        }

        public void Update(string url, ReportType type, string description, string sheet, int batchSize)
        {
            Url = url;
            Type = type;
            Description = description;
            Sheet = sheet;
            BatchSize = batchSize;
        }
    }
}
