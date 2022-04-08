using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertImagesConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50500, 0, N'ImageConfigurations', NULL, NULL),
                         (50501, 0, N'GoodCategoryImageDirectory', NULL, 50500),
                         (50502, 0, N'GoodImageDirectory', NULL, 50500),
                         (50503, 3, N'ImageSizeLimitInKbytes', NULL, 50500)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50501, N'images/category'),
                         (50502, N'images/good'),
                         (50503, N'500')

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
