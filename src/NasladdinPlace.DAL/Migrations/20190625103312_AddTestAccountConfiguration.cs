using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddTestAccountConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60000, 0, N'Test account settings', NULL, NULL)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60001, 2, N'Is payment card verification required', NULL, 60000)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60002, 0, N'Test user name', NULL, 60000)

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (60001, N'True')

                    INSERT INTO ConfigurationValues(KeyId, Value)
                    VALUES (60002, N'79990000001')

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

                    DELETE FROM ConfigurationValues
                    WHERE KeyId IN (60000, 60001, 60002)

                    DELETE FROM ConfigurationKeys
                    WHERE Id IN (60000, 60001, 60002)

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
