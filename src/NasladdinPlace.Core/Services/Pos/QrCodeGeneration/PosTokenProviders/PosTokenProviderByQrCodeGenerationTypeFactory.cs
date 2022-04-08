using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class PosTokenProviderByQrCodeGenerationTypeFactory : IPosTokenProviderByQrCodeGenerationTypeFactory
    {
        private readonly IPosTokenProvider _statelessPosTokenProvider;
        private readonly IPosTokenProvider _statefulPosTokenProvider;

        public PosTokenProviderByQrCodeGenerationTypeFactory(
            IPosTokenProvider statelessPosTokenProvider,
            IPosTokenProvider statefulPosTokenProvider,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (statelessPosTokenProvider == null)
                throw new ArgumentNullException(nameof(statefulPosTokenProvider));
            if (statefulPosTokenProvider == null)
                throw new ArgumentNullException(nameof(statefulPosTokenProvider));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _statelessPosTokenProvider = statelessPosTokenProvider;
            _statefulPosTokenProvider = statefulPosTokenProvider;
        }
        
        public IPosTokenProvider Create(PosQrCodeGenerationType qrCodeGenerationType)
        {
            switch (qrCodeGenerationType)
            {
                case PosQrCodeGenerationType.Static:
                    return _statefulPosTokenProvider;
                case PosQrCodeGenerationType.Dynamic:
                    return _statelessPosTokenProvider;
                default:
                    throw new ArgumentOutOfRangeException(nameof(qrCodeGenerationType), qrCodeGenerationType, null);
            }
        }
    }
}