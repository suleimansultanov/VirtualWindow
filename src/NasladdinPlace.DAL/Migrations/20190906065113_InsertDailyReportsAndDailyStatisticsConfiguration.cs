using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertDailyReportsAndDailyStatisticsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20500, 0, N'DailyReports', NULL, NULL),
                         (20501, 1, N'DailyReportsStartMoscowTime', NULL, 20500),
                         (50007, 1, N'DailyStatisticsTimeFrom', NULL, 50000),
                         (50008, 1, N'DailyStatisticsTimeUntil', NULL, 50000),
                         (50009, 3, N'DailyStatisticsDaysAgo', NULL, 50000)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20501, N'00:10:00'),
                         (50007, N'00:00:00'),
                         (50008, N'23:59:00'),
                         (50009, N'1')

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
