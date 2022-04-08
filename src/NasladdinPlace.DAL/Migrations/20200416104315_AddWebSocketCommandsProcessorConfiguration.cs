using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddWebSocketCommandsProcessorConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (800, 0, N'WebSocketCommandsExecutionProcessor', NULL, NULL),
                           (801, 3, N'DistinctCommandsIdCountLimit', NULL, 800),
                           (802, 3, N'CommandWaitingTimeoutInMilliseconds', NULL, 800)
                          
                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (801, N'10'),
                           (802, N'3000')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (801, 802);
                    DELETE FROM ConfigurationKeys WHERE Id IN (800, 801, 802);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
