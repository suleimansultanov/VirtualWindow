using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Pos.Status
{
    public interface IPosConnectionStatusDetector
    {
        PosConnectionInfo Detect(int posId);
    }
}