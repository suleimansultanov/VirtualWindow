using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class PosTokenProviderByPosIdProvider : IPosTokenProviderByPosIdProvider
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosTokenProviderByQrCodeGenerationTypeFactory _posTokenProviderByQrCodeGenerationTypeFactory;

        public PosTokenProviderByPosIdProvider(
            IUnitOfWorkFactory unitOfWorkFactory, 
            IPosTokenProviderByQrCodeGenerationTypeFactory posTokenProviderByQrCodeGenerationTypeFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posTokenProviderByQrCodeGenerationTypeFactory == null)
                throw new ArgumentNullException(nameof(posTokenProviderByQrCodeGenerationTypeFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
            _posTokenProviderByQrCodeGenerationTypeFactory = posTokenProviderByQrCodeGenerationTypeFactory;
        }
        
        public Task<IPosTokenProvider> ProvideAsync(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pos = unitOfWork.PointsOfSale.GetById(posId);
                
                if (pos == null)
                    throw new NotSupportedException($"Pos with id {posId} does not found.");

                return Task.FromResult(_posTokenProviderByQrCodeGenerationTypeFactory.Create(pos.QrCodeGenerationType));
            }
        }
    }
}