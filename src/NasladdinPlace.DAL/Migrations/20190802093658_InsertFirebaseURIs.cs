using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertFirebaseURIs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50200, 0, N'CloudServiceUris', NULL, NULL),
                         (50201, 0, N'FirebaseCloudMessagingApiUrl', NULL, 50200),
                         (50202, 0, N'FirebaseTokenApiUrl', NULL, 50200)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50201, N'https://fcm.googleapis.com'),
                         (50202, N'https://iid.googleapis.com')

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
