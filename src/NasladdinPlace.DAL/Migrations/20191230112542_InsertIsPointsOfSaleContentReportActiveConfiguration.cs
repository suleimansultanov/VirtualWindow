using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertIsPointsOfSaleContentReportActiveConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20503, 2, N'PointsOfSaleContentReportAgentIsActive', NULL, 20500)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20503, N'True')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (20503);
                    DELETE FROM ConfigurationKeys WHERE Id IN (20503);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
