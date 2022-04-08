using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Spreadsheet.Helpers;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Api.Services.Spreadsheet.Uploader.Contracts;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers
{
    public class ApproachesHolderProvider : IApproachesHolderProvider
    {
        private readonly ApproachesHolder _approachesDataCache;
        private readonly IApproachesUploader _approachesUploader;
        private readonly TimeSpan _cacheLifeTime;

        public ApproachesHolderProvider(IApproachesUploader approachesUploader, TimeSpan cacheLifeTime)
        {
            _approachesUploader = approachesUploader;
            _approachesDataCache = new ApproachesHolder(DateTime.UtcNow);
            _cacheLifeTime = cacheLifeTime;
        }

        public async Task UploadOrCacheRecords(IEnumerable<IReportRecord> newRecords)
        {
            if (_approachesDataCache.Records.Any())
            {
                if (_approachesDataCache.ChangedDateTime > DateTime.UtcNow.Add(-_cacheLifeTime))
                {
                    _approachesDataCache.Records.AddRange(newRecords);
                }
                else
                {
                    await _approachesUploader.UploadAsync(_approachesDataCache.Records);
                    _approachesDataCache.Records.Clear();
                    _approachesDataCache.AddRecords(newRecords);
                }
            }
            else
            {
                _approachesDataCache.AddRecords(newRecords);
            }
        }
    }
}