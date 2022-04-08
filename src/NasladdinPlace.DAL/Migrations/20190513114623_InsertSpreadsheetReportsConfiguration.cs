using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertSpreadsheetReportsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20000, 0, N'SpreadsheetReports', NULL, NULL),
                         (20001, 1, N'PointsOfSaleContentUpdateInterval', NULL, 20000),
                         (20002, 1, N'DailyPurchaseStatisticsStartExportMoscowTime', NULL, 20000)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20001, N'00:20:00'),
                         (20002, N'10:00:00')

                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20100, 0, N'SpreadsheetsUploader', NULL, NULL),
                         (20101, 3, N'SpreadsheetsUploaderBatchSize', NULL, 20100),
                         (20102, 1, N'SpreadsheetsUploaderDelayBeforeRetryInMinutes', NULL, 20100),
                         (20103, 3, N'SpreadsheetsUploaderPermittedRetryCount', NULL, 20100)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20101, N'250'),
                         (20102, N'00:10:00'),
                         (20103, N'5')
                         
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
