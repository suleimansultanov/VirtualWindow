using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddRegistrationUserBonuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @verifyPhoneNumberBonusType INT = 0
                
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                
                BEGIN TRY
                BEGIN TRANSACTION
                  CREATE TABLE #usersWithoutRegistrationBonuses (
                    UserId INT
                  )
                
                  INSERT INTO #usersWithoutRegistrationBonuses (UserId)
                  SELECT U.Id
                  FROM Users U
                  WHERE RegistrationCompletionDate IS NOT NULL
                    AND NOT EXISTS(
                     SELECT *
                     FROM UsersBonuses UB
                     WHERE UB.UserId = U.Id
                       AND UB.Type = @verifyPhoneNumberBonusType
                    )
                    AND RegistrationInitiationDate > DATEADD(DAY, -8, GETUTCDATE())
                    AND TotalBonus <= 50
                
                  UPDATE U
                  SET TotalBonus = TotalBonus + 50
                  FROM Users U
                  INNER JOIN #usersWithoutRegistrationBonuses UWRB ON UWRB.UserId = U.Id
                
                  INSERT UsersBonuses (Value, UserId, DateCreated, Type)
                  SELECT 50, UserId, GETUTCDATE(), @verifyPhoneNumberBonusType
                  FROM #usersWithoutRegistrationBonuses
                
                  DROP TABLE #usersWithoutRegistrationBonuses
                
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
