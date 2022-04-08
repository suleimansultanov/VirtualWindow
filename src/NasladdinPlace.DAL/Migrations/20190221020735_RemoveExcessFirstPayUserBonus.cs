using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RemoveExcessFirstPayUserBonus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                DECLARE @firstPayBonusType INT = 1

                                DECLARE @listOfCheckItemStatus TABLE (checkItemStatus int)
                                INSERT @listOfCheckItemStatus(checkItemStatus) VALUES(2),(5)

                                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                                   BEGIN TRANSACTION
                                      BEGIN TRY
                                        ;WITH UsersWithoutFirstPayBonusRecord (UserId) AS (
                                            SELECT PO.UserId FROM PosOperations PO
                                                WHERE EXISTS(
                                                    SELECT * FROM CheckItems CKI
                                                    WHERE CKI.PosOperationId = PO.Id AND CKI.Status IN (SELECT  checkItemStatus FROM @listOfCheckItemStatus)) 
                                            AND NOT EXISTS(SELECT * FROM UsersBonuses UB WHERE UB.UserId = PO.UserId AND UB.Type = @firstPayBonusType)
                                            GROUP BY PO.UserId)

                                        INSERT UsersBonuses (Value, UserId, DateCreated, Type)
                                         SELECT 50, UserId, GETUTCDATE(), @firstPayBonusType FROM UsersWithoutFirstPayBonusRecord
                                                                                
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
