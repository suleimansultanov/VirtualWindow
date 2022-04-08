using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPosStateChartsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (10109, 3, N'PosStateChartMeasurementDefaultPeriodInMinutes', NULL, 10100),
						 (10110, 3, N'PosStateChartRefreshFrequencyInSeconds', NULL, 10100),
						 (10111, 0, N'PosStateChartDateTimeDisplayFormat', NULL, 10100)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (10109, N'30'),
						 (10110, N'60'),
						 (10111, N'dd.MM.yyyy HH:mm')

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
