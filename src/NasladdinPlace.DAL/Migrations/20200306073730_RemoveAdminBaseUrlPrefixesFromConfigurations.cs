using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RemoveAdminBaseUrlPrefixesFromConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    UPDATE ConfigurationValues SET Value = '/PointsOfSale/{0}/LabeledGoods/Identification' WHERE KeyId = 601

                    UPDATE ConfigurationValues SET Value = N'/GoodsMoving/DocumentGoodsMoving/{0}' WHERE KeyId = 603

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
