using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertVirtualCatalogPageSizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50403, 3, N'VirtualCatalogPageSize', NULL, 50400),
                         (50404, 3, N'VirtualCategoriesPageSize', NULL, 50400),
                         (50405, 0, N'DefaultImagePath', NULL, 50400)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50403, N'15'),
                         (50404, N'10'),
                         (50405, N'images/default/default-food-image.webp')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (50403, 50404, 50405);
                    DELETE FROM ConfigurationKeys WHERE Id IN (50403, 50404, 50405);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
