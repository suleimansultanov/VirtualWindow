using NasladdinPlace.Core.Services.PosScreenResolution.Contracts;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PosScreenResolution
{
    public class PosScreenResolutionUpdater : IPosScreenResolutionUpdater
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosScreenResolutionUpdater(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task UpdateAsync(int posId, Models.ScreenResolution resolution)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                await UpdateAsync(unitOfWork, posId, resolution);
            }
        }

        public async Task UpdateAsync(IUnitOfWork unitOfWork, int posId, Models.ScreenResolution resolution)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            var realTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);

            realTimeInfo.UpdatableScreenResolution.Update(resolution);

            await unitOfWork.CompleteAsync();
        }
    }
}