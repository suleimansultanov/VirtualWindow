using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddSmsRuSenderConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20600, 0, N'SmsRuApiSettings', NULL, NULL),
                         (20601, 0, N'SenderName', NULL, 20600),
                         (20602, 0, N'ApiId', NULL, 20600),
                         (20603, 0, N'SendApiRequestUrl', NULL, 20600),
                         (20604, 3, N'MinimumPositiveBalance', NULL, 20600)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20601, N'Nasladdin'),
                         (20602, N'BF084B1C-B946-A757-3B04-56156AC6DBD3'),
                         (20603, N'https://sms.ru/sms/send'),
                         (20604, N'1000')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (20601, 20602, 20603, 20604);
                    DELETE FROM ConfigurationKeys WHERE Id IN (20600, 20601, 20602, 20603, 20604);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
