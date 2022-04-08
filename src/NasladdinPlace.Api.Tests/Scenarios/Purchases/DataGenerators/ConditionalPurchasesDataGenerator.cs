using FluentAssertions.Extensions;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Tests.Scenarios.Purchases.DataGenerators
{
    public class ConditionalPurchasesDataGenerator
    {
        private const int DefaultPosOperationId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultFirstLabeledGoodId = 1;
        private const int DefaultSecondLabeledGoodId = 2;

        public DateTime PosOperationDate { get; set; }
        public int ExpectedResult { get; set; }
        public PosOperationStatus Status { get; set; }
        public IEnumerable<CheckItem> CheckItems { get; set; }
        public CheckItemStatus ExpectedStatus { get; set; }

        public ConditionalPurchasesDataGenerator()
        {
            CheckItems = new Collection<CheckItem>();
        }


        private static ConditionalPurchasesDataGenerator MakeDataGenerator(DateTime posOperationDate, int expectedResult,
            PosOperationStatus status, IEnumerable<CheckItem> checkItems, CheckItemStatus expectedStatus)
        {
            return new ConditionalPurchasesDataGenerator
            {
                PosOperationDate = posOperationDate,
                ExpectedResult = expectedResult,
                Status = status,
                CheckItems = checkItems,
                ExpectedStatus = expectedStatus
            };
        }

        public delegate ConditionalPurchasesDataGenerator GetGeneratorFunc();

        public static IEnumerable<GetGeneratorFunc> GetUnverifiedTestCases()
        {
	        yield return () => MakeDataGenerator( 15.April( 2011 ), 0, PosOperationStatus.Completed, new Collection<CheckItem>
	        {
		        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
			        DefaultPosOperationId,
			        10M ),
		        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
			        DefaultPosOperationId, 25M )
	        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( 15.April( 2011 ), 0, PosOperationStatus.Paid, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( 15.April( 2011 ), 0, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( DateTime.UtcNow, 1, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( DateTime.UtcNow, 2, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( 15.April( 2011 ), 1, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				 MakeDataGenerator( DateTime.UtcNow, 0, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M, true ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Deleted );

	        yield return () =>
				 MakeDataGenerator( 15.April( 2011 ), 1, PosOperationStatus.Opened, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M, true ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Deleted );
        }

        public static IEnumerable<GetGeneratorFunc> GetPaidTestCases()
        {
	        yield return () =>
				MakeDataGenerator( 15.April( 2011 ), 2, PosOperationStatus.Completed, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.Unverified );

	        yield return () =>
				MakeDataGenerator( 15.April( 2011 ), 1, PosOperationStatus.Paid, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M )
		        }, CheckItemStatus.PaidUnverified );

	        yield return () =>
				MakeDataGenerator( 15.April( 2011 ), 1, PosOperationStatus.Paid, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M, true )
		        }, CheckItemStatus.PaidUnverified );

	        yield return () =>
				MakeDataGenerator( 15.April( 2011 ), 0, PosOperationStatus.Paid, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M, true ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Unverified, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M ),
		        }, CheckItemStatus.PaidUnverified );

	        yield return () =>
				MakeDataGenerator( DateTime.UtcNow, 0, PosOperationStatus.Paid, new Collection<CheckItem>
		        {
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId,
				        DefaultPosOperationId,
				        10M, true ),
			        CreateCheckItem( DefaultPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId,
				        DefaultPosOperationId, 25M, true )
		        }, CheckItemStatus.PaidUnverified );
        }

        public static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId,
            int posOperationId, decimal price, bool isModifiedByAdmin = false)
        {
            var checkItemBuilder = CheckItem.NewBuilder(
                    posId,
                    posOperationId,
                    1,
                    labeledGoodId,
                    1)
                .SetPrice(price)
                .SetStatus(status);

            if (isModifiedByAdmin)
                checkItemBuilder.MarkAsModifiedByAdmin();
                
            return checkItemBuilder.Build();
        }
    }
}
