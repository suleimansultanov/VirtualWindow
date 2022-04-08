using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertAdminPageLinkFormats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (404, 0, N'AdminPosDetailsPageUrl ', NULL, 50005),
                         (405, 0, N'AdminUsersListPageUrl  ', NULL, 50005)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (404, N'/PointsOfSale/EditPos/{0}'),
                         (405, N'/Users?userId={0}')
                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION
        
                    DELETE FROM ConfigurationValues WHERE KeyId IN (404, 405);
                    DELETE FROM ConfigurationKeys WHERE Id IN (404, 405);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
