using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIntegrationTestsVerificationCodeConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60300, 0, N'IntegrationTestsVerificationCodeConfiguration', NULL, NULL)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60301, 0, N'IntegrationTestsVerificationCodeInboxUserName', NULL, 60300),
                           (60302, 0, N'IntegrationTestsVerificationCodeInboxPassword', NULL, 60300),
                           (60303, 3, N'IntegrationTestsVerificationCodeInboxPort', NULL, 60300),
                           (60304, 0, N'IntegrationTestsVerificationCodeInboxUrl', NULL, 60300),
                           (60305, 4, N'IntegrationTestsVerificationCodeSearchRecordsLimit', NULL, 60300),
                           (60306, 4, N'IntegrationTestsVerificationCodeEmailsReadingAttemptsNumber', NULL, 60300),
                           (60307, 4, N'IntegrationTestsVerificationCodeSearchAttempts', NULL, 60300),
                           (60308, 1, N'IntegrationTestsVerificationCodeSearchRepetitionInterval', NULL, 60300)
                            

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (60301, N'smscodes@nasladdin.com'),
                           (60302, N'4u6j0HUIL0Yr'),
                           (60303, N'993'),
                           (60304, N'imap.yandex.ru'),
                           (60305, N'10'),
                           (60306, N'3'),
                           (60307, N'3'),
                           (60308, N'00:00:15')

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
                    WHERE KeyId BETWEEN 60301 AND 60306

                    DELETE FROM ConfigurationKeys
                    WHERE Id BETWEEN 60300 AND 60308

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
