using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateUserTotalBonusColumnBaseUsersBonusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                                   BEGIN TRANSACTION
                                      BEGIN TRY
                                        UPDATE U 
                                            SET U.TotalBonus = UB.Value 
                                        FROM Users AS U INNER JOIN UsersBonuses AS UB ON U.Id = UB.UserId
                                        
                                   COMMIT TRANSACTION

                                      END TRY
                                      BEGIN CATCH
                                          ROLLBACK TRANSACTION
                                      END CATCH");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
