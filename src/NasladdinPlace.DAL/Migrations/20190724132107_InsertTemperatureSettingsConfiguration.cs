using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertTemperatureSettingsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (10100, 0, N'TemperatureSettings', NULL, NULL),
                         (10101, 3, N'LowerNormalTemperature', NULL, 10100),
						 (10102, 3, N'UpperNormalTemperature', NULL, 10100),
						 (10103, 3, N'CalcAverageTemperatureEveryInMinutes', NULL, 10100),
						 (10104, 3, N'NotifyIfNoTemperatureUpdatesMoreThanInMinutes', NULL, 10100),
						 (10105, 3, N'PreventNotifyAbnormalTemperatureAfterAdminOperationInMinutes', NULL, 10100),
						 (10106, 3, N'PosTemperatureStateCheckIntervalInMinutes', NULL, 10100)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (10101, N'0'),
						 (10102, N'6'),
						 (10103, N'35'),
						 (10104, N'10'),
						 (10105, N'15'),
						 (10106, N'10')

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
