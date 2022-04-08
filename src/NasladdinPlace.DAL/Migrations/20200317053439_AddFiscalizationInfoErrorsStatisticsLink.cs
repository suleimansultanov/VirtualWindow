using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddFiscalizationInfoErrorsStatisticsLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (50015, 0, N'FiscalizationInfoTotalErrorsStatisticsLink', NULL, 50000),
                           (50016, 0, N'FiscalizationInfoDailyErrorsStatisticsLink', NULL, 50000)

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (50015, N'{0}/GoodsMoving?OperationMode={1}&HasFiscalizationInfoErrors={2}'),
                           (50016, N'{0}/GoodsMoving?OperationDateFrom={1}&OperationDateUntil={2}&OperationMode={3}&HasFiscalizationInfoErrors={4}')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (50015, 50016);
                    DELETE FROM ConfigurationKeys WHERE Id IN (50015, 50016);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
