using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertCheckManagerNotificationMessagesConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (300, 0, N'CheckManagerNotificationMessages', NULL, NULL),
                         (301, 0, N'RefundMessageFormat', NULL, 300),
						 (302, 2, N'IsPermittedToNotifyAboutRefund', NULL, 300),
						 (303, 0, N'AdditionOrVerificationMessageFormat', NULL, 300),
                         (304, 2, N'IsPermittedToNotifyAboutAdditionOrVerification', NULL, 300)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (301, N'Чек №{0} от {1:d MMM} был проверен. Возврат {2:0} руб ожидайте в теч. 3 раб.дн. Остались вопросы - свяжитесь с нами +7(495)481-37-68'),
						 (302, N'false'),
						 (303, N'Чек №{0} от {1:d MMM} был проверен. C карты дополнительно спишется {2:0} руб. Остались вопросы - свяжитесь с нами +7(495)481-37-68'),
                         (304, N'false')

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
