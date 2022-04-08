using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPosDeviceConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (50700, 0, N'PosDeviceConfigurations', NULL, NULL),
                           (50701, 5, N'MinBatteryLevel', NULL, 50700),
                           (50702, 3, N'TimeIntervalBetweenNextStartInSeconds', NULL, 50700),
                           (50703, 3, N'InactiveTimeoutInMinutes', NULL, 50700),
                           (50704, 3, N'RepeatNotificationInMinutes', NULL, 50700),
                           (50705, 3, N'OngoingPurchaseActivityMonitorTimeoutInSeconds', NULL, 50700)

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (50701, N'20'),
                           (50702, N'60'),
                           (50703, N'2'),
                           (50704, N'10'),
                           (50705, N'20')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (50701, 50702, 50703, 50704, 50705);
                    DELETE FROM ConfigurationKeys WHERE Id IN (50700, 50701, 50702, 50703, 50704, 50705);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
