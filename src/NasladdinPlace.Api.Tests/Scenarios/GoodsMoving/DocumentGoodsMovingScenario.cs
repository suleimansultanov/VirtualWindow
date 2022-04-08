using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.Logging;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Tests.Scenarios.GoodsMoving
{
    public class DocumentGoodsMovingScenario : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int SecondPosId = 2;
        private const int DefaultSecondGoodId = 2;
        private const int DefaultCurrencyId = 1;
        private const int DefaultThreadsCount = 8;
        private const int MiddleOfThreads = DefaultThreadsCount / 2;

        private AccountingBalancesController _wsAccountingBalancesController;
        private IServiceProvider _serviceProvider;
        private AutoResetEvent _queueIsEmptyForFirstPosWaitHandle;
        private WsCommandsQueueProcessor _processor;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new PromotionSettingsDataSet());

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            _serviceProvider = TestServiceProviderFactory.Create();

            _queueIsEmptyForFirstPosWaitHandle = new AutoResetEvent(false);

            _processor = CreateWsCommandsQueueProcessorAndSubscribeOnEvent(_queueIsEmptyForFirstPosWaitHandle);

            var mockPosRealTimeInfo = _serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();
            mockPosRealTimeInfo.Setup(p => p.GetOrAddById(DefaultPosId)).Returns(new PosRealTimeInfo(DefaultPosId)
            {
                CommandsQueueProcessor = _processor
            });

            _wsAccountingBalancesController = _serviceProvider.GetRequiredService<AccountingBalancesController>();
        }

        [Test]
        public void Synchronize_NoGoodIsTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 1, documentGoodsMovingLabeledGoodsCount: 6);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 3, balanceAtEnd: 3, income: 0, outcome: 0);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 3, balanceAtEndCount: 3);
        }

        [Test]
        public void Synchronize_AddedLabeledGoodsNoGoodIsTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            CreateAdditionalLabeledGoods();
            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 12",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 80 11 60 60 00 02 05 2A 98 AB 14",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 12);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 6, income: 0, outcome: 0);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 6);
        }

        [Test]
        public void Synchronize_TwoGoodsAreTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            CreateAdditionalLabeledGoods();
            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 12",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 10);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 4, income: 0, outcome: 2);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 4);
        }

        [Test]
        public void Synchronize_TwoGoodsArePut_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            var additioinalLabelsInPos = new List<Tuple<int, string>>
            {
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 15"),
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 16")
            };

            CreateAdditionalLabeledGoods(additioinalLabelsInPos);

            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 12",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 80 11 60 60 00 02 05 2A 98 AB 14",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1",
                "E2 80 11 60 60 00 02 05 2A 98 AB 15",
                "E2 80 11 60 60 00 02 05 2A 98 AB 16"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 14);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 8, income: 2, outcome: 0);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 8);
        }

        [Test]
        public void Synchronize_ThreeGoodsArePutAndTwoGoodsAreTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            var additioinalLabelsInPos = new List<Tuple<int, string>>
            {
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 15"),
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 16"),
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 17")
            };

            CreateAdditionalLabeledGoods(additioinalLabelsInPos);

            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 80 11 60 60 00 02 05 2A 98 AB 14",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1",
                "E2 80 11 60 60 00 02 05 2A 98 AB 15",
                "E2 80 11 60 60 00 02 05 2A 98 AB 16",
                "E2 80 11 60 60 00 02 05 2A 98 AB 17"

            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 13);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 7, income: 3, outcome: 2);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 7);
        }

        [TestCase(true)]
        [TestCase(true, true)]
        [TestCase(false)]
        public void Synchronize_ThreeGoodsArePutAndTwoGoodsAreTakenTheSynchronizeCommandSentInMultiplyThreads_ShouldReturnCorrectResultOfDocumentGoodsMoving(
            bool useTheSameMessage, bool insertNewMessageInTheMiddle = false)
        {
            var additioinalLabelsInPos = new List<Tuple<int, string>>
            {
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 15"),
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 16"),
                Tuple.Create(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 17")
            };

            CreateAdditionalLabeledGoods(additioinalLabelsInPos);

            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 80 11 60 60 00 02 05 2A 98 AB 14",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1",
                "E2 80 11 60 60 00 02 05 2A 98 AB 15",
                "E2 80 11 60 60 00 02 05 2A 98 AB 16",
                "E2 80 11 60 60 00 02 05 2A 98 AB 17"

            };

            var wsMessage = CreateWsMessage(labelsInPos, DefaultPosId);
            var newWsMessage = CreateWsMessage(labelsInPos, DefaultPosId);

            var tasks = new Collection<Task>();
            for (var i = 0; i < DefaultThreadsCount; i++)
            {
                var webSocket = CreateWebSocket();
                var message = useTheSameMessage ? wsMessage : CreateWsMessage(labelsInPos, DefaultPosId);
                var isInsertNewMessage = insertNewMessageInTheMiddle && i == MiddleOfThreads;

                var wsControllerInvoker = _serviceProvider.GetRequiredService<IWsControllerInvoker>();

                tasks.Add(Task.Factory.StartNew(() => wsControllerInvoker.InvokeAsync(webSocket, isInsertNewMessage ? newWsMessage : message).GetAwaiter().GetResult()));
            }

            Task.WaitAll(tasks.ToArray());

            _queueIsEmptyForFirstPosWaitHandle.WaitOne();

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 13);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 7, income: 3, outcome: 2);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 7);
        }

        [Test]
        public void Synchronize_AllGoodsAreTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            CreateAdditionalLabeledGoods();

            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string>();

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 6);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 6, balanceAtEnd: 0, income: 0, outcome: 6);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 6, balanceAtEndCount: 0);
        }

        [Test]
        public void Synchronize_AddedUntiedLabeledGoodsNoGoodIsTaken_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            MarkOpenedPosOperationsAsPendingCompletion();

            var labeledGoods = new List<LabeledGood>
            {
                LabeledGood.NewOfPosBuilder(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 12").Build(),
                LabeledGood.NewOfPosBuilder(SecondPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 13").Build()
            };

            Context.LabeledGoods.AddRange(labeledGoods);
            Context.SaveChanges();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 80 11 60 60 00 02 05 2A 98 AB 12",
                "E2 80 11 60 60 00 02 05 2A 98 AB 13",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 8);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 3, balanceAtEnd: 5, income: 2, outcome: 0);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 3, balanceAtEndCount: 5);
            CheckUntiedDocumentGoodsMovingLabeledGoodsCount(expectedUntiedLabeledGoods: 1);
        }

        [Test]
        public void Synchronize_TakenUntiedLabeledGoods_ShouldReturnCorrectResultOfDocumentGoodsMoving()
        {
            MarkOpenedPosOperationsAsPendingCompletion();

            var labeledGoods = new List<LabeledGood>
            {
                LabeledGood.NewOfPosBuilder(DefaultPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 12").Build(),
                LabeledGood.NewOfPosBuilder(DefaultPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 13").Build()
            };

            Context.LabeledGoods.AddRange(labeledGoods);
            Context.SaveChanges();

            var labelsInPos = new Collection<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            };

            MakeLabelsSynchronization(labelsInPos);

            CheckCorrectnesCountOfDocumentsAndRelatedItems(documentsCount: 1, tablePartItemsCount: 2, documentGoodsMovingLabeledGoodsCount: 8);
            CheckCorrectnesDocumentGoodsMovingTableItems(balanceAtBegigning: 5, balanceAtEnd: 3, income: 0, outcome: 2);
            CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(balanceAtBegigningCount: 5, balanceAtEndCount: 3);
            CheckUntiedDocumentGoodsMovingLabeledGoodsCount(expectedUntiedLabeledGoods: 1);
        }

        private static WsMessage CreateWsMessage(Collection<string> labelsInPos, int posId)
        {
            var wsControllerRoute = new WsControllerRoute("accountingBalances", "synchronize");
            var body = new JObject(
                new JProperty("posId", posId),
                new JProperty("labels", new JArray(labelsInPos.Select(lbl => new JValue(lbl)))),
                new JProperty("commandId", Guid.NewGuid()));
            var wsMessage = new WsMessage(wsControllerRoute, body);
            return wsMessage;
        }

        private WsCommandsQueueProcessor CreateWsCommandsQueueProcessorAndSubscribeOnEvent(EventWaitHandle queueIsEmptyWaitHandler)
        {
            var processor = new WsCommandsQueueProcessor(
                10,
                TimeSpan.FromMilliseconds(7000),
                _serviceProvider.GetRequiredService<ILogger>());

            processor.OnQueueProcessed += (sender, eventArgs) =>
            {
                queueIsEmptyWaitHandler.Set();
            };

            return processor;
        }

        private void MakeLabelsSynchronization(ICollection<string> labels)
        {
            _wsAccountingBalancesController.Synchronize(new PosAccountingBalancesDto
            {
                Labels = labels,
                PosId = DefaultPosId
            }).Wait();
        }

        private void MarkOpenedPosOperationsAsPendingCompletion()
        {
            using (var context = ProvideNewContext())
            {
                var openedPosOperations = context.PosOperations
                    .Where(po => po.Status == PosOperationStatus.Opened)
                    .ToImmutableList();
                var pendingCompletionPosOperations = openedPosOperations.Select(po =>
                {
                    po.MarkAsPendingCompletion();
                    po.Mode = PosMode.GoodsPlacing;
                    return po;
                }).ToImmutableList();
                context.UpdateRange(pendingCompletionPosOperations);
                context.SaveChanges();
            }
        }

        private void CheckCorrectnesCountOfDocumentsAndRelatedItems(int documentsCount, int tablePartItemsCount, int documentGoodsMovingLabeledGoodsCount)
        {
            Context.DocumentsGoodsMoving.Count().Should().Be(documentsCount);
            Context.DocumentGoodsMovingTableItems.Count().Should().Be(tablePartItemsCount);
            Context.DocumentGoodsMovingLabeledGoods.Count().Should().Be(documentGoodsMovingLabeledGoodsCount);
        }

        private void CheckCorrectnesDocumentGoodsMovingTableItems(int balanceAtBegigning, int balanceAtEnd, int income, int outcome)
        {
            var docuentGoodsMovingTableItems = Context.DocumentGoodsMovingTableItems.AsNoTracking().ToImmutableList();

            docuentGoodsMovingTableItems.Sum(ti => ti.BalanceAtBegining).Should().Be(balanceAtBegigning);
            docuentGoodsMovingTableItems.Sum(ti => ti.BalanceAtEnd).Should().Be(balanceAtEnd);
            docuentGoodsMovingTableItems.Sum(ti => ti.Income).Should().Be(income);
            docuentGoodsMovingTableItems.Sum(ti => ti.Outcome).Should().Be(outcome);
        }

        private void CheckCorrectnesDocumentGoodsMovingLabeledGoodsCount(int balanceAtBegigningCount, int balanceAtEndCount)
        {
            var docuentGoodsMovingLabeledGoods = Context.DocumentGoodsMovingLabeledGoods.AsNoTracking().ToImmutableList();

            docuentGoodsMovingLabeledGoods.Count(lg => lg.BalanceType == BalanceType.AtBegining).Should().Be(balanceAtBegigningCount);
            docuentGoodsMovingLabeledGoods.Count(lg => lg.BalanceType == BalanceType.AtEnd).Should().Be(balanceAtEndCount);
        }

        private void CheckUntiedDocumentGoodsMovingLabeledGoodsCount(int expectedUntiedLabeledGoods)
        {
            var tablePart = Context.DocumentsGoodsMoving.Include(doc => doc.TablePart).First().TablePart;

            tablePart.Count(dti => dti.GoodId == null).Should().Be(expectedUntiedLabeledGoods);
        }

        private void CreateAdditionalLabeledGoods(IReadOnlyCollection<Tuple<int, string>> additioinalLabelsInPosCollection = null)
        {
            var labelsInPosCollection = new List<Tuple<int, string>>
            {
                Tuple.Create(DefaultPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 12"),
                Tuple.Create(DefaultPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 13"),
                Tuple.Create(DefaultPosId, "E2 80 11 60 60 00 02 05 2A 98 AB 14")
            };

            if (additioinalLabelsInPosCollection != null)
                labelsInPosCollection.AddRange(additioinalLabelsInPosCollection);

            foreach (var (posId, label) in labelsInPosCollection)
            {
                var labeledGood = LabeledGood.NewOfPosBuilder(posId, label)
                    .TieToGood(
                        DefaultSecondGoodId,
                        new LabeledGoodPrice(5M, DefaultCurrencyId),
                        new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
                    )
                    .Build();

                Context.LabeledGoods.Add(labeledGood);
            }

            Context.SaveChanges();
        }

        private static WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(new MemoryStream(), true, null, TimeSpan.FromMinutes(1));
        }
    }
}
