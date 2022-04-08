using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPayment3DResultUrlsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (70000, 0, N'Payment3DsResultUrls', NULL, NULL),
                         (70001, 0, N'Payment3DsResultUrlsSuccess', NULL, 70000),
                         (70002, 0, N'Payment3DsResultUrlsFailure', NULL, 70000)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (70001, N'Results3Ds/Success'),
                         (70002, N'Results3Ds/Failure?error=')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (70001, 70002);
                    DELETE FROM ConfigurationKeys WHERE Id IN (70000, 70001, 70002);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");

        }
    }
}
