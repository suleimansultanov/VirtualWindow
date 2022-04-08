using System;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders
{
    public class PosByStatefulTokenProvider : IPosByTokenProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosByStatefulTokenProvider(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public Task<ValueResult<Models.Pos>> TryProvideByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            return TryProvideByTokenAuxAsync(token);
        }

        private async Task<ValueResult<Models.Pos>> TryProvideByTokenAuxAsync(string token)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByQrCodeAsync(token);

                return pos == null
                    ? ValueResult<Models.Pos>.Failure($"Token is incorrect {token}.")
                    : ValueResult<Models.Pos>.Success(pos);
            }
        }
    }
}