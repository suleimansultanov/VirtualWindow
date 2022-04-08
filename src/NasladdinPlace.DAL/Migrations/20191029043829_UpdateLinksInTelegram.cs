using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateLinksInTelegram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    UPDATE ConfigurationValues
                    SET [Value] = '{0}/GoodsMoving?OperationDateUntil={1}&OperationStatus={2}&OperationStatusFilterType={3}&OperationMode={4}&TotalPrice={5}&TotalPriceFilterType={6}'
                    WHERE  KeyId = 50002

                    UPDATE ConfigurationValues
                    SET [Value] = '{0}/GoodsMoving?OperationDateUntil={1}&OperationMode={2}&HasUnverifiedCheckItems={3}'
                    WHERE  KeyId = 50003

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    UPDATE ConfigurationValues
                    SET [Value] = '{0}/Checks?OperationDateUntil={1}&OperationStatus={2}&OperationStatusFilterType={3}&OperationMode={4}&TotalPrice={5}&TotalPriceFilterType={6}'
                    WHERE  KeyId = 50002

                    UPDATE ConfigurationValues
                    SET [Value] = '{0}/Checks?OperationDateUntil={1}&OperationMode={2}&HasUnverifiedCheckItems={3}'
                    WHERE  KeyId = 50003

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
