using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders
{
    public class PosByStatelessTokenProvider : IPosByTokenProvider
    {
        private readonly IStatelessPosTokenManager _statelessPosTokenManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosByStatelessTokenProvider(
            IStatelessPosTokenManager statelessPosTokenManager,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (statelessPosTokenManager == null)
                throw new ArgumentNullException(nameof(statelessPosTokenManager));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _statelessPosTokenManager = statelessPosTokenManager;
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
            if (!_statelessPosTokenManager.TryGetPosIdIfTokenValid(token, out var posId))
                return ValueResult<Models.Pos>.Failure($"Token {token} is incorrect.");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);

                return pos == null
                    ? ValueResult<Models.Pos>.Failure($"Pos with id {posId} does not exists.")
                    : ValueResult<Models.Pos>.Success(pos);
            }
        }
    }
}