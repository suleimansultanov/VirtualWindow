using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    public interface IMediaContentToPosPlatformRepository
    {
        Task<MediaContentToPosPlatform> GetLastMediaContentToPosPlatformByScreenType(PosScreenType screenType);
    }
}
