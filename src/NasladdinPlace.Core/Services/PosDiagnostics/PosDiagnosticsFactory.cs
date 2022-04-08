using System;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.PosDiagnostics.Contracts;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;

namespace NasladdinPlace.Core.Services.PosDiagnostics
{
    public class PosDiagnosticsFactory : IPosDiagnosticsFactory
    {
        private readonly IPosInteractor _posInteractor;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PosDiagnosticsFactory(IPosInteractor posInteractor, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _posInteractor = posInteractor;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public IPosDiagnostics Create(PosDiagnosticsType posDiagnosticsType, PosDiagnosticsContext context)
        {
            switch (posDiagnosticsType)
            {
                case PosDiagnosticsType.Purchase:
                    return new PurchasePosDiagnostics(
                        _posInteractor,
                        _unitOfWorkFactory,
                        context
                    );
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(posDiagnosticsType),
                        posDiagnosticsType,
                        $"{nameof(PosDiagnosticsType)} has not been supported yet."
                     );
            }
        }
    }
}