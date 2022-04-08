using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class ChangeModeInPosOperationsWithUsersWhereUserNameIsEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION

                        DECLARE @PurchaseMode int = 0
                        DECLARE @GoodsPlacingMode int = 2

                        UPDATE po
                        SET po.Mode = @GoodsPlacingMode
                        FROM PosOperations AS po 
                        INNER JOIN Users AS u
                        ON po.UserId = u.Id
                        WHERE u.UserName LIKE '%_@__%.__%' AND po.Mode = @PurchaseMode

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
