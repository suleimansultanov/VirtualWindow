using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Logging;
using NasladdinPlace.Payment.Services;

namespace NasladdinPlace.Core.Services.Check.Factory
{
    public static class CheckManagerFactory
    {
        public static ICheckManager Create(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            var paymentService = serviceProvider.GetRequiredService<IPaymentService>();
            var refundCalculator = serviceProvider.GetRequiredService<ICheckRefundCalculator>();
            var checkOnlineManager = serviceProvider.GetRequiredService<ICheckOnlineManager>();
            var paymentDescriptionPrinter = serviceProvider.GetRequiredService<IPaymentDescriptionPrinter>();
            var purchaseCompletionManager = serviceProvider.GetRequiredService<IPurchaseCompletionManager>();
            var operationTransactionManager = serviceProvider.GetRequiredService<IOperationTransactionManager>();
            var checkManagerBonusPointsHelper = serviceProvider.GetRequiredService<ICheckManagerBonusPointsHelper>();
            var transactionCreationService = serviceProvider.GetRequiredService<IPosOperationTransactionCreationUpdatingService>();

            var logger = serviceProvider.GetRequiredService<ILogger>();


            var checkManagerPaymentHelper = new CheckManagerPaymentHelper(
                paymentService,
                paymentDescriptionPrinter,
                checkManagerBonusPointsHelper);

            var checkManagerOperationTransactionWrapper = new CheckManagerOperationTransactionWrapper(unitOfWorkFactory, logger);

            var checkManagerRefundOrDeleteHelper = new CheckManagerRefundOrDeletionHelper(
                refundCalculator, 
                checkManagerPaymentHelper, 
                checkManagerOperationTransactionWrapper,
                checkManagerBonusPointsHelper,
                operationTransactionManager,
                transactionCreationService);

            var checkManager = new CheckManager(
                checkManagerPaymentHelper,
                checkManagerRefundOrDeleteHelper,
                checkManagerOperationTransactionWrapper,
                purchaseCompletionManager,
                checkOnlineManager,
                operationTransactionManager,
                checkManagerBonusPointsHelper);

            return checkManager;
        }
    }
}
