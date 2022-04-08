using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertTacticalDiagnosticsConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (50300, 0, N'TacticalDiagnostics', NULL, NULL),
                         (50301, 1, N'StartMoscowTime', NULL, 50300),
                         (50302, 0, N'PhoneNumber', NULL, 50300),
                         (50303, 0, N'BankingCardCryptogram', NULL, 50300),
                         (50304, 0, N'PosQrCode', NULL, 50300)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (50301, N'06:00:00'),
                         (50302, N'996777661718'),
                         (50303, N'014111111111221102IuUDalV2SXohVf9+dGtsH+hI8W/mgbDyKH9KFZnQTGa/8oSCaFEF54aqR3rpwanlfcRyQhM7ZTebXuRw8vgWvdrkgerDLlcJ2UfVylVHVPUI4TrQ0qGhyx9JmeqGfFlS2vwltZT5IBI9Ci7KAlcuCBHgUc4/epKl3+0Km92LqfBRn0H14spyUT8SZUvVGJOB5eJBlyLzyoQII125KFKc/4fzB8lXTO4Wgj5uXDJcvK/jNh+3TIdXhadUbzw/G+pwsOXNex9XG0gLaDwFslw539PNYXjMK+Ubbdi4KCmZOCYw9GIGSdYeeu/+0pmA/FniWNgJz6XA/6A5KSWxjKHvtw=='),
                         (50304, N'4ea72566-538c-4589-bdaa-2e2f4ea98afc')

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
