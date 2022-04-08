using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateStatisticsConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
				
                  DELETE FROM ConfigurationValues WHERE KeyId IN (50001, 50002, 50003)

			      DELETE FROM ConfigurationKeys WHERE Id IN (50001, 50002, 50003)
				
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50001, 3, N'AuditRequestExpirationHours', NULL, 50000),
                         (50002, 0, N'TotalUnpaidCheckItemsLink', NULL, 50000),
                         (50003, 0, N'DailyUnhandledConditionalPurchasesCountLink', NULL, 50000),
                         (50006, 3, N'UnhandledConditionalPurchaseExpirationHours', NULL, 50000)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50001, N'24'),
                         (50002, N'{0}/Checks?OperationDateUntil={1}&OperationStatus={2}&OperationStatusFilterType={3}&OperationMode={4}&TotalPrice={5}&TotalPriceFilterType={6}'),
                         (50003, N'{0}/Checks?OperationDateUntil={1}&OperationMode={2}&HasUnverifiedCheckItems={3}'),
                         (50006, N'25')

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
