using System;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class StatefulPosTokenProvider : IPosTokenProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public StatefulPosTokenProvider(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task<ValueResult<string>> TryProvidePosTokenAsync(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);

                return pos == null 
                    ? ValueResult<string>.Failure($"Pos with id {posId} does not exist.") 
                    : ValueResult<string>.Success(pos.QrCode);
            }
        }
    }
}