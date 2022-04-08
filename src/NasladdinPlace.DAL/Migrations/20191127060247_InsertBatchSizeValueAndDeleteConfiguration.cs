using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertBatchSizeValueAndDeleteConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (50014, 1, N'DailyStatisticsReportsBeforeStartDelayInMinutes', NULL, 50000)

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (50014, N'00:05:00')

                    DELETE FROM ConfigurationValues WHERE KeyId = 20101;
                    DELETE FROM ConfigurationKeys WHERE Id = 20101;

                    UPDATE ReportsUploadingInfo SET BatchSize = 300;

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
