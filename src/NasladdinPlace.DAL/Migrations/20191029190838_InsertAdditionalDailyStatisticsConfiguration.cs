using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertAdditionalDailyStatisticsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION

                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50010, 0, N'DailyStatisticsBasePurchasesLink', NULL, 50000),
                         (50011, 0, N'DailyStatisticsUsersLazyLink', NULL, 50000),
                         (50012, 0, N'DailyStatisticsUsersNotLazyLink', NULL, 50000),
                         (50013, 0, N'DailyStatisticsPosAbnormalSensorMeasurementCountLink', NULL, 50000)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50010, N'{0}/GoodsMoving?OperationDateFrom={1}&OperationDateUntil={2}&OperationMode={3}'),
                         (50011, N'{0}/Users?LazyUsersFrom={1}&LazyUsersUntil={2}&Type={3}'),
                         (50012, N'{0}/Users?NotLazyUsersFrom={1}&NotLazyUsersUntil={2}'),
                         (50013, N'{0}/PosSensors/GetPosAbnormalSensorMeasurement?PosAbnormalSensorMeasurementDateFrom={1}&PosAbnormalSensorMeasurementDateUntil={2}')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (50010, 50011, 50012, 50013);
                    DELETE FROM ConfigurationKeys WHERE Id IN (50010, 50011, 50012, 50013);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
