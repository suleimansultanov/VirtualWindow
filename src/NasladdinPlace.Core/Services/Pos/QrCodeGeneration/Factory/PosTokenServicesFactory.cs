using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory
{
    public class PosTokenServicesFactory : IPosTokenServicesFactory
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly PosTokenServicesOptions _posTokenServicesOptions;

        public PosTokenServicesFactory(
            IUnitOfWorkFactory unitOfWorkFactory,
            PosTokenServicesOptions posTokenServicesOptions)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posTokenServicesOptions == null)
                throw new ArgumentNullException(nameof(posTokenServicesOptions));
            
            _unitOfWorkFactory = unitOfWorkFactory;
            _posTokenServicesOptions = posTokenServicesOptions;
        }
        
        public IPosTokenProvider CreatePosTokenProvider()
        {
            var statelessPosTokenProvider = CreateStatelessPosTokenProvider();
            var statefulPosTokenProvider = CreateStatefulPosTokenProvider();

            var posTokenProviderByQrCodeGenerationTypeFactory = new PosTokenProviderByQrCodeGenerationTypeFactory(
                statelessPosTokenProvider: statelessPosTokenProvider,
                statefulPosTokenProvider: statefulPosTokenProvider,
                unitOfWorkFactory: _unitOfWorkFactory
            );
            IPosTokenProviderByPosIdProvider posTokenProviderByPosIdProvider = new PosTokenProviderByPosIdProvider(
                _unitOfWorkFactory,
                posTokenProviderByQrCodeGenerationTypeFactory
            );
            posTokenProviderByPosIdProvider = new CachedPosTokenProviderByPosIdProvider(
                posTokenProviderByPosIdProvider: posTokenProviderByPosIdProvider,
                cachePeriod: _posTokenServicesOptions.PosTokenProviderCachePeriod
            );
            
            return new SwitchablePosTokenProvider(posTokenProviderByPosIdProvider);
        }

        public IPosByTokenProvider CreatePosByTokenProvider()
        {
            var posByStatelessTokenProvider = 
                new PosByStatelessTokenProvider(CreateStatelessPosTokenManager(), _unitOfWorkFactory);
            var posByStatefulTokenProvider =
                new PosByStatefulTokenProvider(_unitOfWorkFactory);
            var posByTokenProviders = new List<IPosByTokenProvider>
            {
                posByStatelessTokenProvider,
                posByStatefulTokenProvider
            };
            return new SwitchablePosByTokenProvider(posByTokenProviders);
        }

        private IPosTokenProvider CreateStatelessPosTokenProvider()
        {
            return new StatelessPosTokenProvider(CreateStatelessPosTokenManager());
        }

        private IPosTokenProvider CreateStatefulPosTokenProvider()
        {
            var statefulTokenProvider = new StatefulPosTokenProvider(_unitOfWorkFactory);
            return new StatefulPosTokenWithPrefixProvider(statefulTokenProvider, _posTokenServicesOptions.TokenPrefix);
        }

        private IStatelessPosTokenManager CreateStatelessPosTokenManager()
        {
            return PosStatelessTokenManagerFactory.Create(_posTokenServicesOptions);
        }
    }
}