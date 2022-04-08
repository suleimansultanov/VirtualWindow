using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertBaseApiUrlConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (500, 0, N'NasladdinApiUrls', NULL, NULL),
                         (501, 0, N'BaseApiUrl', NULL, 500)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (501, N'https://api.nasladdin.club')

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
