using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class LeaveThreeUsersAdminsOthersDowngradeToLogisticianRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                        DECLARE @LogisticianRoleId int = (SELECT Id FROM Roles WHERE NormalizedName = 'LOGISTICIAN')
                        DECLARE @AdminRoleId int = (SELECT Id FROM Roles WHERE NormalizedName = 'ADMIN')

						UPDATE UserRoles
						SET RoleId = CASE
										WHEN u.NormalizedUserName IN ('PCMYFRIEND@GMAIL.COM', 'ILUHAESIN@GMAIL.COM', 'EVG.MOSKALENKO@GMAIL.COM',
																	  'EFSOL117@SUPPORTNASLADDIN.ONMICROSOFT.COM', 'BATYRKANOV.NB@GMAIL.COM') 
												THEN @AdminRoleId
										ELSE @LogisticianRoleId
									  END
						FROM UserRoles AS ur
						INNER JOIN Users AS u
						ON ur.UserId = u.Id
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
