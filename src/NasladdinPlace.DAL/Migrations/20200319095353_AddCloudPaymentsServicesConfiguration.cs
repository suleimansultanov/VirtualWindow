using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddCloudPaymentsServicesConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (700, 0, N'CloudPaymentsServices', NULL, NULL),
                           (701, 0, N'CloudKassirInn', NULL, 700),
                           (702, 3, N'CloudKassirTaxationSystem', NULL, 700)
                          
                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (701, N'5048047172'),
                           (702, N'0')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (701, 702);
                    DELETE FROM ConfigurationKeys WHERE Id IN (700, 701, 702);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
