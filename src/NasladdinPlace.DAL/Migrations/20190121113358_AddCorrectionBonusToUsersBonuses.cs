using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddCorrectionBonusToUsersBonuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                   BEGIN TRANSACTION
                     BEGIN TRY
                        ALTER TABLE UsersBonuses ADD OldBonus DECIMAL(18,2) NOT NULL DEFAULT(0)		
						EXECUTE('UPDATE UsersBonuses SET OldBonus=Bonus')		

						DECLARE @maximumBonusesSum DECIMAL(18, 2) = 250

						UPDATE UsersBonuses SET Bonus = @maximumBonusesSum WHERE Bonus > @maximumBonusesSum  

                        DECLARE @transactionTypePaid INT = 0
						DECLARE @transactionTypeRefund INT = 1
                        
                        ;WITH MoneySpentByUsers(UserId, Amount)
						AS
						(
						  SELECT PO.UserId, SUM(
							CASE WHEN Bti.Type = @transactionTypePaid THEN Amount
								 WHEN Bti.Type = @transactionTypeRefund THEN -Amount
								 ELSE 0
								 END
						  )
						  FROM BankTransactionInfos AS Bti
						  INNER JOIN PosOperations PO on Bti.PosOperationId = PO.Id and Bti.PosId = PO.PosId
						  GROUP BY PO.UserId
						), BonusesSpentByUsers(UserId, Amount)
						AS
						(
						  SELECT UserId, SUM(BonusAmount)
						  FROM PosOperations
						  GROUP BY UserId
						), MoneyAndBonusesSpentByAllUsers(UserId, MoneySpent, BonusesSpent)
						AS
						(
						  SELECT Users.Id, ISNULL(MoneySpentByUsers.Amount, 0), ISNULL(BonusesSpentByUsers.Amount, 0)
						  FROM Users
						  LEFT JOIN BonusesSpentByUsers ON BonusesSpentByUsers.UserId = Users.Id
						  LEFT JOIN MoneySpentByUsers ON MoneySpentByUsers.UserId = Users.Id
						)

                        UPDATE UB
						SET Bonus = (
						  CASE WHEN MBU.MoneySpent >= @maximumBonusesSum OR MBU.BonusesSpent > @maximumBonusesSum THEN 0
							   WHEN MBU.BonusesSpent + UB.Bonus > @maximumBonusesSum THEN @maximumBonusesSum - MBU.BonusesSpent
							   ELSE Bonus
							   END
						  )
						FROM UsersBonuses UB
						INNER JOIN MoneyAndBonusesSpentByAllUsers AS MBU ON UB.UserId = MBU.UserId

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
