using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class FixBankTransactionInfosAndCheckItemsBy223Error : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SET TRAN ISOLATION LEVEL SERIALIZABLE

                    BEGIN TRANSACTION
	                    BEGIN TRY 

		                CREATE TABLE #TransactionInfos (
			                PosId INT NOT NULL,
			                PosOperationId INT NOT NULL,
			                BankTransactionId INT NOT NULL,
			                Amount DECIMAL(18,2) NOT NULL,
			                RefundCount INT NULL)

		                DECLARE @BankTransactionInfoError INT = 2
		                DECLARE @BankTransactionInfoRefund INT = 1
		                DECLARE @CheckItemRefunded INT = 3
		                DECLARE @CheckItemPaidUnverified INT = 5
		                DECLARE @Comment NVARCHAR(3000) = N'Возврат системы по сбою 223'

		                INSERT INTO #TransactionInfos
		                SELECT 
			                po.PosId,
			                po.Id AS PosOperationId,
			                bti.BankTransactionId,
			                bti.Amount,
			                RefundCount = CASE 
				                WHEN po.Id IN (18013, 18140, 18155, 18651) THEN 2 ELSE 0
                            END 
		                FROM PosOperations po
		                INNER JOIN BankTransactionInfos bti
			                ON bti.PosOperationId = po.Id
		                WHERE po.Id IN(8309, 18306, 18013, 18140, 18155, 18651, 19244) AND bti.Type = @BankTransactionInfoError
		                GROUP BY po.PosId, po.Id, bti.BankTransactionId, bti.Amount

		                INSERT INTO BankTransactionInfos
		                SELECT 
			                BankTransactionId,
			                Amount,
			                PosId,
			                PosOperationId,
			                @Comment AS Comment,
			                GETUTCDATE() AS DateCreated,
			                @BankTransactionInfoRefund AS Type,
			                NULL AS PaymentCardId
		                FROM #TransactionInfos
		 
		                INSERT INTO BankTransactionInfos
		                SELECT 
			                BankTransactionId,
			                Amount,
			                PosId,
			                PosOperationId,
			                @Comment AS Comment,
			                GETUTCDATE() AS DateCreated,
			                @BankTransactionInfoRefund AS Type,
			                NULL AS PaymentCardId
		                FROM #TransactionInfos WHERE RefundCount = 2

		                UPDATE  ci
			                SET ci.Status = @CheckItemRefunded,
				                ci.IsModifiedByAdmin = 1
		                FROM CheckItems ci 
			                INNER JOIN #TransactionInfos ti
				                ON ti.PosOperationId = ci.PosOperationId
					                AND ci.Status = @CheckItemPaidUnverified  
					                AND ti.PosId = ci.PosId

						DROP TABLE #TransactionInfos

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
