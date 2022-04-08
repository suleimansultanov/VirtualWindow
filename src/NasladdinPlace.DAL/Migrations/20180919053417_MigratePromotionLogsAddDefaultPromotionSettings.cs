using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class MigratePromotionLogsAddDefaultPromotionSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    @"  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                        BEGIN TRANSACTION
	                        BEGIN TRY
                                            
                            DECLARE @FirstNotificationDate DATE

	                        IF EXISTS(SELECT * FROM dbo.UserNotifications)
	                        BEGIN
		                        SELECT
			                        @FirstNotificationDate = u.SendMessageDateTime
		                        FROM dbo.UserNotifications AS u
	                        END
	                        
	                        INSERT INTO dbo.UserNotifications
		                        (NotificationArea, NotificationType, MessageText, PhoneNumber, SendMessageDateTime, Token, UserId)
	                        SELECT
		                        CASE p.PromotionType
			                        WHEN 0 THEN 4	-- PromotionTypeEnum.VerifyPhoneNumber -> NotificationAreaEnum.PromotionVerifyPhoneNumber
			                        WHEN 1 THEN 5	-- PromotionTypeEnum.VerifyPaymentCard -> NotificationAreaEnum.PromotionVerifyPaymentCard
			                        WHEN 2 THEN 6	-- PromotionTypeEnum.FirstPay -> NotificationAreaEnum.PromotionFirstPay
		                        END,
		                        p.NotificationType,
		                        CASE p.PromotionType
			                        WHEN 0 THEN N'Продолжите регистрацию и получите 200 рублей!'
			                        WHEN 1 THEN N'На вашем счете 200 руб! Привяжите карту и начните покупки!'
			                        WHEN 2 THEN N'Сделайте первую покупку и получите 50 руб на счет!'
		                        END,
		                        u.PhoneNumber,
		                        p.SendMessageDateTime,
		                        NULL,
		                        p.UserId
	                        FROM dbo.PromotionLogs AS p
	                        INNER JOIN dbo.Users AS u ON p.UserId = u.Id
	                        WHERE @FirstNotificationDate IS NULL OR p.SendMessageDateTime < @FirstNotificationDate
	                        
	                        --Add default data to promotion settings
	                        DELETE FROM dbo.PromotionSettings

	                        INSERT INTO dbo.PromotionSettings 
		                        (BonusValue, IsEnabled, IsNotificationEnabled, NotificationStartTime, PromotionType)
	                         VALUES (50, 1, 1, '10:30:00', 0),	-- PromotionTypeEnum.VerifyPhoneNumber
	                                (0, 1, 1, '10:35:00', 1),	-- PromotionTypeEnum.VerifyPaymentCard
			                        (50, 1, 1, '11:00:00', 2)	-- PromotionTypeEnum.FirstPay

                        COMMIT TRANSACTION
                                            
                            END TRY
	                        BEGIN CATCH
		                        ROLLBACK TRANSACTION
	                        END CATCH                     
                        "
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
