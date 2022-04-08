using System;
using System.Linq;
using FluentAssertions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Repositories;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.Repositories.LabeledGoodsRepository
{
    public class LabeledGoodRepositoryIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int OtherPosId = 2;
        private const int DefaultGoodId = 1;
        private const int DefaultUserId = 1;
        private const int OtherUserId = 2;
        private const string DefaultLabel = "74 72 61 63 65 00 02 05 51 37 bb e3 b2 6f e2 80 11 00 20 00 77 93 26 f7 08 aac";
        private const int DefaultSize = 25;

        private LabeledGoodRepository _labeledGoodRepository;

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
            
            _labeledGoodRepository = new LabeledGoodRepository(Context);
        }

        [Test]
        public void GetEnabledInPosAsync_NoLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            var result = _labeledGoodRepository.GetEnabledInPosAsync(DefaultPosId).Result;

            result.Should().BeEmpty();
        }


        [Test]
        public void GetEnabledInPosAsync_LabeledGoodOfOtherShopIsGiven_ShouldReturnEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(OtherPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .Build()
            );

            var result = _labeledGoodRepository.GetEnabledInPosAsync(DefaultPosId).Result;

            result.Should().BeEmpty();
        }
        
        [Test]
        public void GetEnabledInPosAsync_UserLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            Insert(PosOperation.NewOfUserAndPosBuilder(1, DefaultPosId).Build());

            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .MarkAsUsedInPosOperation(Context.PosOperations.First().Id)
                    .Build()
                );

            var result = _labeledGoodRepository.GetEnabledInPosAsync(DefaultPosId).Result;

            result.Should().BeEmpty();
        }
        
        [Test]
        public void GetEnabledInPosAsync_SuitableLabeledGoodIsGiven_ShouldReturnLabeledGood()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .Build()
            );

            var result = _labeledGoodRepository.GetEnabledInPosAsync(DefaultPosId).Result;

            result.Should().HaveCount(1);
        }

        [Test]
        public void GetOverdueInShopAsync_NoLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.Zero).Result;
            result.Should().BeEmpty();
        }

        [Test]
        public void GetOverdueInShopAsync_UserLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            Insert(PosOperation.NewOfUserAndPosBuilder(1, DefaultGoodId).Build());
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .MarkAsUsedInPosOperation(1)
                    .Build()
                );

            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.Zero).Result;

            result.Should().BeEmpty();
        }

        [Test]
        public void GetOverdueInShopAsync_LabeledGoodOfOtherShopIsGiven_ShouldReturnEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(OtherPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .Build()
                );

            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.Zero).Result;

            result.Should().BeEmpty();
        }

        [Test]
        public void GetOverdueInShopAsync_FreshLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(0M, GetCurrencyId()),
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1))
                    )
                    .Build()
            );

            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.Zero).Result;

            result.Should().BeEmpty();
        }

        [Test]
        public void GetOverdueInShopAsync_OverdueLabeledGoodIsGiven_ShouldReturnLabeledGood()
        {
            var labeledGood = LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                .TieToGood(
                    DefaultGoodId,
                    new LabeledGoodPrice(0M, GetCurrencyId()),
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1))
                )
                .Build();

            labeledGood.SetProperty(nameof(LabeledGood.ExpirationDate), DateTime.UtcNow.AddDays(-1));
            
            Insert(labeledGood);

            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.Zero).Result;

            result.Should().HaveCount(1);
        }
        
        [Test]
        public void GetOverdueInShopAsync_OneDayLeftLabeledGoodIsGivenAndDeltaIsTwoDay_ShouldReturnLabeledGood()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(0M, GetCurrencyId()),
                        ExpirationPeriod.FromNowTill(DateTime.UtcNow.AddDays(1))
                    )
                    .Build()
            );

            var result = _labeledGoodRepository.GetEnabledOverdueInPosAsync(DefaultPosId, TimeSpan.FromDays(2)).Result;

            result.Should().HaveCount(1);
        }

        [Test]
        public void GetTakenByUserIncludingGoodsAndMakersAndPricesAsync_NoLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            var result = _labeledGoodRepository.GetEnabledTakenByUserIncludingGoodsAndMakersAndPricesAsync(DefaultUserId).Result;
            
            result.Should().BeEmpty();
        }

        [Test]
        public void
            GetTakenByUserIncludingGoodsAndMakersAndPricesAsync_SuitableLabeledGoodIsGiven_ShouldReturnLabeledGood()
        {
            Insert(PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId).Build());
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(100M, GetCurrencyId()),
                        new ExpirationPeriod()
                    )
                    .MarkAsUsedInPosOperation(1)
                    .Build()
            );
            
            var result = _labeledGoodRepository.GetEnabledTakenByUserIncludingGoodsAndMakersAndPricesAsync(DefaultUserId).Result;
            
            result.Should().HaveCount(1);

            var labeledGood = result[0];
            labeledGood.Good.Should().NotBeNull();
            labeledGood.Good.Maker.Should().NotBeNull();
        }
        
        [Test]
        public void
            GetTakenByUserIncludingGoodsAndMakersAndPricesAsync_LabeledGoodOfOtherUserIsGiven_ShouldReturnEmpty()
        {
            Insert(PosOperation.NewOfUserAndPosBuilder(OtherUserId, DefaultPosId).Build());
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(100M, GetCurrencyId()), new ExpirationPeriod())
                    .MarkAsUsedInPosOperation(1)
                    .Build()
            );
            
            var result = _labeledGoodRepository.GetEnabledTakenByUserIncludingGoodsAndMakersAndPricesAsync(DefaultUserId).Result;
            
            result.Should().BeEmpty();
        }

        [Test]
        public void
            GetTakenByUserIncludingGoodsAndMakersAndPricesAsync_LabeledGoodInShopIsGiven_ShouldReturnEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(DefaultGoodId, new LabeledGoodPrice(0M, GetCurrencyId()), new ExpirationPeriod())
                    .Build()
                );
            
            var result = _labeledGoodRepository.GetEnabledTakenByUserIncludingGoodsAndMakersAndPricesAsync(DefaultUserId).Result;
            
            result.Should().BeEmpty();
        }

        [Test]
        public void GetUntiedFromGoodByShop_NoLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            var result = _labeledGoodRepository.GetEnabledUntiedFromGoodByPos(DefaultPosId).Result;

            result.Should().BeEmpty();
        }

        [Test]
        public void GetUntiedFromGoodByShop_SuitableLabeledGoodIsGiven_ShouldReturnLabeledGood()
        {
            Insert(LabeledGood.OfPos(DefaultPosId, DefaultLabel));

            var result = _labeledGoodRepository.GetEnabledUntiedFromGoodByPos(DefaultPosId).Result;

            result.Should().HaveCount(1);
        }

        [Test]
        public void GetUntiedFromGoodByShop_LabeledGoodOfOtherShopIsGiven_ShouldReturnEmpty()
        {
            Insert(LabeledGood.OfPos(OtherPosId, DefaultLabel));

            var result = _labeledGoodRepository.GetEnabledUntiedFromGoodByPos(DefaultPosId).Result;

            result.Should().BeEmpty();
        }
        
        [Test]
        public void GetUntiedFromGoodByShop_UserSuitableLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            Insert(PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId).Build());
            
            var labeledGood = LabeledGood.OfPos(DefaultPosId, DefaultLabel);
            labeledGood.MarkAsUsedInPosOperation(1);
            
            Insert(labeledGood);

            var result = _labeledGoodRepository.GetEnabledUntiedFromGoodByPos(DefaultPosId).Result;

            result.Should().BeEmpty();
        }

        [Test]
        public void GetEnabledUntiedIncludingPosAsync_UntiedLabeledGoodIsGiven_ShouldReturnExpectedResult()
        {
            Insert(LabeledGood.OfPos(DefaultPosId, DefaultLabel));
            var untiedLabeledGoods = _labeledGoodRepository.GetEnabledUntiedIncludingPosAsync().Result;
            untiedLabeledGoods.Should().NotBeEmpty();
            untiedLabeledGoods.Should().HaveCount(1);
        }

        [Test]
        public void GetEnabledUntiedIncludingPosAsync_TieLabeledGoodIsGiven_ShouldReturnEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(0M, GetCurrencyId()),
                        new ExpirationPeriod()
                    )
                    .Build()
            );
            var untiedLabeledGoods = _labeledGoodRepository.GetEnabledUntiedIncludingPosAsync().Result;
            untiedLabeledGoods.Should().BeEmpty();
        }

        [Test]
        public void GetAllTiedInPosAsync_TieLabeledGoodIsGiven_ShouldReturnNotEmpty()
        {
            Insert(
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(0M, GetCurrencyId()),
                        new ExpirationPeriod()
                    )
                    .Build()
            );
            var untiedLabeledGoods = _labeledGoodRepository
                .GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory();
            untiedLabeledGoods.Should().NotBeEmpty();
        }

        [Test]
        public void GetAllTiedInPosAsync_TieLabeledGoodIsNotGiven_ShouldReturnEmpty()
        {
            var untiedLabeledGoods = _labeledGoodRepository
                .GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory();
            untiedLabeledGoods.Should().BeEmpty();
        }

        private void Insert<T>(params T[] entities) where T : class
        {
            Seeder.Seed(entities);
        }

        private int GetCurrencyId()
        {
            return Context.Currencies.First().Id;
        }
    }
}