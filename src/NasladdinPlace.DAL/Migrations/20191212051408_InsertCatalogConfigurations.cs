using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertCatalogConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50400, 0, N'Catalog', NULL, NULL),
                         (50401, 3, N'CatalogPageSize', NULL, 50400),
                         (50402, 3, N'CategoriesPageSize', NULL, 50400)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50401, 10),
                         (50402, 5)

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
                
                    DELETE FROM ConfigurationValues WHERE KeyId IN (50401, 50402);
                    DELETE FROM ConfigurationKeys WHERE Id IN (50400, 50401, 50402);

                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
