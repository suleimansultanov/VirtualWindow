using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPaymentCardsToBankTransactionInfos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRANSACTION
                
                  BEGIN TRY
                
                    UPDATE BankTransactionInfos
                    SET PaymentCardId = (
                      SELECT DISTINCT U.ActivePaymentCardId
                      FROM Users U
                      INNER JOIN PosOperations PO on U.Id = PO.UserId
                      WHERE BankTransactionInfos.PosOperationId = PO.Id)
                    WHERE PosOperationId IN (
                      SELECT PO.Id
                      FROM Users U
                      INNER JOIN PosOperations PO on U.Id = PO.UserId
                      WHERE ActivePaymentCardId IS NOT NULL
                    )
                
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
