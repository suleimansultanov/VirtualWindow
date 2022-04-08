using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Status;
using System;

namespace NasladdinPlace.UI.DependencyInjection
{
    public class EmptyPosConnectionStatusDetector : IPosConnectionStatusDetector
    {
        public PosConnectionInfo Detect(int posId)
        {
            throw new NotImplementedException();
        }
    }
}
