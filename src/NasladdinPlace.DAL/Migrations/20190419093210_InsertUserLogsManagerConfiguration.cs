using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertUserLogsManagerConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (200, 0, N'UserLogsManager', NULL, NULL),
                         (201, 1, N'UserLogsManagerOldLogsDeletionInterval', NULL, 200),
                         (202, 1, N'UserLogsManagerLogsStoragePeriod', NULL, 200)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (201, N'1.00:00:00'),
                         (202, N'30.00:00:00')
                
                  PRINT 'SUCCESS'
                
                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  PRINT 'ERROR'
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
