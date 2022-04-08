using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertDailyStatisticsReportConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50000, 0, N'DailyStatistics', NULL, NULL),
                         (50001, 0, N'DailySumOfBonusAndDiscountLink', NULL, 50000),
                         (50002, 0, N'DailyConditionalIncomeAndMoneyLossLink', NULL, 50000),
                         (50003, 0, N'DailyConditionalPurchasesCountLink', NULL, 50000),
                         (50004, 3, N'UserLazyDaysCount', NULL, 50000),
                         (50005, 0, N'AdminPageBaseUrl', NULL, 50000)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50001, N'{0}/Checks?OperationDateFrom={1}&OperationDateUntil={2}'),
                         (50002, N'{0}/Checks?OperationDateFrom={1}&OperationDateUntil={2}&HasUnverifiedCheckItems={3}&OperationStatus={4}'),
                         (50003, N'{0}/Checks?OperationDateFrom={1}&OperationDateUntil={2}&HasUnverifiedCheckItems={3}'),
                         (50004, N'30'),
                         (50005, N'https://online.nasladdin.club')

                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
