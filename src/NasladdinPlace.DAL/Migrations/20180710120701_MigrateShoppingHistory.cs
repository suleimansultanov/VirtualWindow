using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class MigrateShoppingHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

                  BEGIN TRANSACTION

                      BEGIN TRY
                    
                        INSERT INTO CheckItems (LabeledGoodId, Price, CurrencyId, PosOperationId, PosId, Action, IsHardToDetect, GoodId)
                        SELECT GoodHistoryRecords.LabeledGoodId, L.Price, L.CurrencyId, GoodHistoryRecords.PosOperationId, PO.PosId, 0, 0, L.GoodId
                        FROM GoodHistoryRecords
                        INNER JOIN LabeledGoods L on GoodHistoryRecords.LabeledGoodId = L.Id
                        INNER JOIN PosOperations PO on GoodHistoryRecords.PosOperationId = PO.Id and GoodHistoryRecords.PosId = PO.PosId
                        WHERE Type = 1 AND IsFinal = 1 AND L.GoodId IS NOT NULL
                    
                        UPDATE PosOperations
                        SET DatePaid = DateCompleted, PosOperations.Status = 4
                        WHERE DateCompleted IS NOT NULL
                    
                        COMMIT TRANSACTION
                    
                      END TRY
                      BEGIN CATCH
                        ROLLBACK TRANSACTION
                      END CATCH"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
