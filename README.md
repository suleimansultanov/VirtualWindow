##### ВАЖНО! Запуская у себя локально копию боевой БД, пропатчи ее скриптом ниже!!! Иначе рискуешь слезть в боевые механизмы ПРОДА #############

######## СКРИПТ ДЛЯ ПАТЧА БОЕВОЙ БД ПЕРЕД ИСПОЛЬЗОВАНИЕМ НА ДРУГИХ СТЕНДАХ --
declare @baseAdminProdPattern nvarchar(1000)
declare @baseApiUrl nvarchar(1000)
declare @baseAdminUrl nvarchar(1000)
declare @serverMode nvarchar(1)

-- УКАЖИТЕ ЗДЕСЬ УРЛЫ ДО ВАШЕГО БЭКЕНДА

set @baseApiUrl = N'http://localhost:5000' -- Пример для API: https://sync.testapi.nasladdin.club
set @baseAdminUrl = N'http://localhost:5001' -- Пример для Админки: https://sync.testonline.nasladdin.club
set @serverMode = N'2' -- ServerMode { Production = 0, Beta = 1, Development = 2 }

begin tran

-- поисковый паттерн для замены продовского урла админки
set @baseAdminProdPattern = N'https://online.nasladdin.club'

-- обновляем урл апи
update [ConfigurationValues] set Value = @baseApiUrl where KeyId = 501

-- обновляем режим сервера
update [ConfigurationValues] set Value = @serverMode where KeyId = 502

-- обновляем урл админки
update [ConfigurationValues] set Value = @baseAdminUrl where KeyId = 50005

declare @keyId int

-- заменяем начало админки
set @keyId = 601
update [ConfigurationValues] set Value = Replace((select [Value] from [ConfigurationValues] where KeyId = @keyId), @baseAdminProdPattern, @baseAdminUrl) where KeyId = @keyId

-- заменяем начало админки
set @keyId = 603
update [ConfigurationValues] set Value = Replace((select [Value] from [ConfigurationValues] where KeyId = @keyId), @baseAdminProdPattern, @baseAdminUrl) where KeyId = @keyId

-- ломается ссылка на файл для ростика (посещаемость)
set @keyId = 10301
update [ConfigurationValues] set Value = Replace((select [Value] from [ConfigurationValues] where KeyId = @keyId), N'f28m7mNSpnnEKUapWc', N'f28m7mNSpnnEKUapWcREMOVE') where KeyId = @keyId

-- меняется урл к файлам отчетов на 1 бета файл
UPDATE ReportsUploadingInfo SET Url = 'https://docs.google.com/spreadsheets/d/1XKp2Vl_uwBCHvRhihD7zl9YA-f28m7mNSpnnEKUapWc'

commit tran

####### КОНЕЦ. СКРИПТ ДЛЯ ПАТЧА БОЕВОЙ БД ПЕРЕД ИСПОЛЬЗОВАНИЕМ НА ДРУГИХ СТЕНДАХ


########### Настройка проекта ###########
1. Скачиваем проект
2. Через SSMS версии не ниже 17.9.1 восстанавливаем базу NasladdinDb (Databases -> Import Data-tier Application...)
3. Открываем проект
4. Получаем актуальные конфиги UserSecrets
5. Нажимаем ПКМ на NasladdinPlace.Api
6. Выбираем пункт Manage User Secrets
7. Вставляем настройки из актуального SecretsApi
8. Прописываем AdminPageBaseUrl, который открывается при запуске приложения NasladdinPlace.Api
9. Прописываем подключение к БД
10. Нажимаем ПКМ на NasladdinPlace.UI
11. Выбираем пункт Manage User Secrets
12. Вставляем настройки из актуального SecretsUI
13. Прописываем DefaultBaseUrl
14. Прописываем подключение к БД

PS: Если нужно завести нового пользователя в AdminUser прописываем свой email и password. Новый пользователь создастся автоматически. Для работы запускаем NasladdinPlace.Api и NasladdinPlace.UI

########### Описание API ###########

Создание пользователя (после вызова придет смс на указанный номер телефона):
Метод: POST 
URL: /api/account/createUser
Content-Type: application/json
Модель запроса: Registration
Модель ответа: id пользователя

Проверка номера телефона:
Метод: POST
URL: /api/account/verifyPhoneNumber
Content-Type: application/json
Модель запроса: VerifyPhoneNumber
Модель ответа: UserPassword

Получение токена:
Метод: POST
URL: /connect/token
Content-Type: application/x-www-form-urlencoded
Модель запроса: grant_type=password&username=996777276646&password=8070ef1a-03dc-49d8-96c2-719c96ef6cfc&scope=openid+email+name+profile+roles
Модель ответа: AuthPayload

Отправка содержимого холодильника
Метод: POST
URL: api/labeledGoods/verifyTransaction
Модель запроса: PlantContent
Модель ответа: PlantContentResponse

Запрос на отправку положенных/взятых товаров:
Метод: PUT
URL: api/labeledGoods/assignTransaction
Модель запроса: InOutLabeledGoods
Модель ответа: нет (код 200)

Запрос на получение товаров, которые находятся внутри холодильника
Метод: GET
URL: api/labeledGoods/insidePlant/{plantId}
Модель ответа: [LabeledGood]

Запрос на получение товаров на складе
Метод: GET
URL: api/labeledGoods/unassigned
Модель ответа: [LabeledGood]

Запрос на получение содержимого холодильника
Протокол: WebSocket
Модель: WebSocketGetContentRequest

Запрос на открытие двери холодильника
Протокол: WebSocket
Модель: WebSocketOpenPlantDoor

Запрос на получение отчета по операциям
Метод: POST
URL: api/ShoppingStatistics/operationsReport
Модель запроса: Criteria
Модель ответа: OperationsReport

Запрос на получение отчета по пользователям
Метод: POST
URL: api/ShoppingStatistics/usersPurchasingInfoReport
Модель запроса: Criteria
Модель ответа: UsersPurchasingInfoReport

Запрос на получение отчета по товарам
Метод: POST
URL: api/ShoppingStatistics/goodsPurchasingInfoReport
Модель запроса: Criteria
Модель ответа: GoodsPurchasingInfoReport

Запрос на получение отчета по меченным товарам
Метод: POST
URL: api/ShoppingStatistics/labeledGoodsPurchasingInfoReport
Модель запроса: Criteria
Модель ответа: LabeledGoodsPurchasingInfoReport

########### Модели ###########

Registration 
{
   string PhoneNumber // обязательное поле
}

VerifyPhoneNumber
{
	string PhoneNumber // обязательное поле
	string Code // обязательное поле
}

UserPassword
{
	string Password // обязательное поле
}

AuthPayload
{
	string access_token
	string scope
	string token_type
	int expires_in
	string id_token
}

########### Примеры (JSON) ###########

Registration (модель запроса):
{
	"phoneNumber": "996777276646"
}

VerifyPhoneNumber:
{
	"phoneNumber": "996777276646",
	"code": "593573"
}

PlantContent:
{
	"plantId": 1, 
	"labels": []
}

PlantContentResponse
{
	"synchronized": true
	"unsynchronizedLabels": []
}

InOutLabeledGoods
{
	"userActionOUT": [], // взятые товары
	"userActionIN": [], // положенные товары
	"param": {
		"ID": 1 // Id холодильника
	}
}

LabeledGood 
{
	// TODO: write description
}

WebSocketGetContentRequest 
{
	"H": "PlantHub",
	"M": "getContent”,
	“A”: null
}

WebSocketOpenPlantDoor
{
	/// Открытие правой двери: {"H":"PlantHub”,”M”:”openRightDoor”,”A”:null}
	/// Открытие левой двери: {"H":"PlantHub”,”M”:”openLeftDoor”,”A”:null}
	/// Закрытие левой двери: {"H":"PlantHub”,”M”:”closeLeftDoor”,”A”:null}
	/// Закрытие правой двери: {"H":"PlantHub”,”M”:”closeRightDoor”,”A”:null}
}

LabeledGoodsPurchasingInfoReport
{
    "content": [
        {
            "goodName": "Good 1",
            "goodDescription": "Some description",
            "artical": "Some article",
            "purchasedDate": "20/07/2017 04:59"
        },
        {
            "goodName": "Good 2",
            "goodDescription": "Some description",
            "artical": "Some article",
            "purchasedDate": "20/07/2017 04:59"
        }
    ],
    "page": 1,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
}

GoodsPurchasingInfoReport
{
    "content": [
        {
            "goodId": 1,
            "goodName": "Good 1",
            "goodDescription": "Some description",
            "purchaseQuantity": 1
        },
        {
            "goodId": 2,
            "goodName": "Good 2",
            "goodDescription": "Some description",
            "purchaseQuantity": 1
        }
    ],
    "page": 1,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
}

UsersPurchasingInfoReport
{
    "content": [
        {
            "userId": 1,
            "phoneNumber": "PCmyfriend@gmail.com",
            "operationsQuantity": 14,
            "purchasedQuantity": 0,
            "purchasedAmount": 0,
            "currency": ""
        },
        {
            "userId": 5,
            "phoneNumber": "996777276646",
            "operationsQuantity": 4,
            "purchasedQuantity": 2,
            "purchasedAmount": 320,
            "currency": "руб"
        }
    ],
    "page": 1,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
}

OperationsReport
{
    "content": [
        {
            "plantName": "УХ2",
            "dateCompleted": "01/08/2017 09:34",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "УХ2",
            "dateCompleted": "01/08/2017 09:33",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:19",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:14",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:14",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:03",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:03",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:02",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 09:02",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        },
        {
            "plantName": "Test Plant",
            "dateCompleted": "01/08/2017 08:45",
            "userPhoneNumber": "PCmyfriend@gmail.com",
            "purchaseQuantity": 0,
            "purchaseAmount": 0,
            "currency": ""
        }
    ],
    "page": 1,
    "totalPages": 2,
    "hasPreviousPage": false,
    "hasNextPage": true
}

Criteria
{
	"page": 1,
	"pageSize": 10,
    "dateTimeFrom": "20/01/2017 10:00",
    "dateTimeUntil": "20/01/2018 15:25",
    "plantId": 1
}

AuthPayload:
{
  "scope": "openid email profile roles",
  "token_type": "Bearer",
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IlFQR0RHWThUQlFNRkE0UERGRUtKLUFXQkVMSDJIUU1FU1YyTkNGSUEiLCJ0eXAiOiJKV1QifQ.eyJ1bmlxdWVfbmFtZSI6IjE4NzAyOTkzOTMxODE2OTIiLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6Ijc5NjJlZjVjLTFmM2EtNDU1MS04OTU4LTU5ZmUwMWM1NzIxNyIsImp0aSI6Ijk4YjE0OGZmLTBiYzItNDVmMC05ZTllLTIwYTE0ZjVmOTJjNSIsInVzYWdlIjoiYWNjZXNzX3Rva2VuIiwic2NvcGUiOlsib3BlbmlkIiwiZW1haWwiLCJwcm9maWxlIiwicm9sZXMiXSwic3ViIjoiZWFkNzM5MWUtMDc2ZS00OWY2LTgzMmQtNmIwOWM2MzUyZGJlIiwiYXVkIjoiaHR0cDovL3NtdWthbmJldG92YS0wMDEtc2l0ZTEuZnRlbXB1cmwuY29tLyIsIm5iZiI6MTQ5NzAxNjA2OCwiZXhwIjoxNTI4NTUyMDY4LCJpYXQiOjE0OTcwMTYwNjgsImlzcyI6Imh0dHA6Ly9zbXVrYW5iZXRvdmEtMDAxLXNpdGUxLmZ0ZW1wdXJsLmNvbS8ifQ.D83xoYL5vUDGlNLSzhVqplnCFFAqpti6LMxUFrrOwEMYlfGUpu1QE1e-72XVmP4nt-3Gp3d-OoEb6fTB6apHkCKfNVnfP2RXKLZNA98ZLx4MUo13HpUnCcHsgyUCXz2rDJClt2Z_H47lXJcL5lo8KE3V6pzJhXFT2Pw2Zzi4POBss057mWgxA1P-vJZj-KJi_VxUacuUnD-WkD2vm3CUYXRGNRUIdm7xACCs0pAVOs1iAvWwGvW20XASp_BNrw0-naWwB8ZDBREVIM_Tq232zX6nglbcF3Q1czucAuI9XUFvzmrdr8SwXj2kC_0MTJqh8QkBN-nQx_bHUoKPu-FJPQ",
  "expires_in": 31536000,
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IlFQR0RHWThUQlFNRkE0UERGRUtKLUFXQkVMSDJIUU1FU1YyTkNGSUEiLCJ0eXAiOiJKV1QifQ.eyJ1bmlxdWVfbmFtZSI6IjE4NzAyOTkzOTMxODE2OTIiLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6Ijc5NjJlZjVjLTFmM2EtNDU1MS04OTU4LTU5ZmUwMWM1NzIxNyIsInN1YiI6ImVhZDczOTFlLTA3NmUtNDlmNi04MzJkLTZiMDljNjM1MmRiZSIsImp0aSI6ImZmZjFjOTQ0LTViNDctNGY1YS05OTJhLTdlYmMyMmRkOGY2NiIsInVzYWdlIjoiaWRlbnRpdHlfdG9rZW4iLCJhdF9oYXNoIjoiQzMtaW1YZy1QWUgwdmRVblZZbmUyZyIsIm5iZiI6MTQ5NzAxNjA2OCwiZXhwIjoxNDk3MDE3MjY4LCJpYXQiOjE0OTcwMTYwNjgsImlzcyI6Imh0dHA6Ly9zbXVrYW5iZXRvdmEtMDAxLXNpdGUxLmZ0ZW1wdXJsLmNvbS8ifQ.bt3D6kL5MzQQYmsBZqvxuXPzq5QEuPy-LcYhjEYKvQGh4SHsg1Qm03KNGD_ttEHyj2Mhxw8OzRxFXU-NlyM1aeFrHtBiwFPBwRxI7oqFbETju4KWJn6qeRzmXpMqwDJwhhrmLZPnEzfo4gkZJC_9R2UzvfGTv38lT4RAc-k5F_Iylsiir784mP9b6lUKCqXMn7R24JBm0QPCoZ80wxFphE9Cz7dIvl7xdpJST1OFPvNyUehPf4d1G99G94Ie4YRf9J6mvK5rE1RSs-i8Y5BcsRkzRcl2BJ1l_WePvCsy7syww_9T13DZYBZoW-A9puHN12hE64VXSVuw5DaphqA7qA"
}

########### Скрипты для демонстрации ###########

##### Скрипт привязки карты и токена для пользователя и добавление завершенной неоплаченной операции с тремя позициями (одна из них неподтверждена) #####

DECLARE @PosId INT = 1
DECLARE @CheckItemUnpaidStatus INT = 1
DECLARE @CheckItemUnverifiedStatus INT = 0
DECLARE @UserId INT = 1
DECLARE @PosOperationCompletedStatus INT = 2

INSERT INTO [dbo].[PaymentCards]
           ([Token]
           ,[PaymentSystem]
           ,[UserId]
           ,[CryptogramSource])
     VALUES
           (N'477BBA133C182267FE5F086924ABDC5DB71F77BFC27F01F2843F2CDC69D89F05'
           ,0
           ,@UserId
           ,0)

UPDATE Users SET ActivePaymentCardId = (SELECT Id FROM [dbo].[PaymentCards] WHERE UserId = @UserId)
WHERE Id = @UserId


INSERT INTO [dbo].[PosOperations]
           ([PosId]
           ,[UserId]
           ,[DateCompleted]
           ,[DateSentForVerification]
           ,[DateStarted]
           ,[DatePaid]
           ,[BonusAmount]
           ,[Status]
           ,[Brand]
           ,[Mode]
           ,[AuditRequestDateTime]
           ,[AuditCompletionDateTime])
     VALUES
           (@PosId
           ,@UserId
           ,DATEADD(SECOND, 5, GETUTCDATE())
           ,null
           ,GETUTCDATE()
           ,null
           ,0
           ,@PosOperationCompletedStatus
           ,1
           ,0
           ,null
           ,null)

DECLARE @PosOperationId INT 
SELECT @PosOperationId= Id FROM PosOperations WHERE UserId = @UserId
INSERT INTO [dbo].[CheckItems]
           ([LabeledGoodId]
           ,[Price]
           ,[CurrencyId]
           ,[PosOperationId]
           ,[PosId]
           ,[GoodId]
           ,[IsModifiedByAdmin]
           ,[Status]
           ,[DiscountAmount])
     VALUES
           (12
           ,250
           ,1
           ,@PosOperationId
           ,@PosId
           ,3
           ,0
           ,@CheckItemUnpaidStatus
           ,0)
INSERT INTO [dbo].[CheckItems]
           ([LabeledGoodId]
           ,[Price]
           ,[CurrencyId]
           ,[PosOperationId]
           ,[PosId]
           ,[GoodId]
           ,[IsModifiedByAdmin]
           ,[Status]
           ,[DiscountAmount])
     VALUES
           (13
           ,300
           ,1
           ,@PosOperationId
           ,@PosId
           ,3
           ,0
           ,@CheckItemUnpaidStatus
           ,0)
		   INSERT INTO [dbo].[CheckItems]
           ([LabeledGoodId]
           ,[Price]
           ,[CurrencyId]
           ,[PosOperationId]
           ,[PosId]
           ,[GoodId]
           ,[IsModifiedByAdmin]
           ,[Status]
           ,[DiscountAmount])
     VALUES
           (13
           ,180
           ,1
           ,@PosOperationId
           ,@PosId
           ,3
           ,0
           ,@CheckItemUnverifiedStatus
           ,0)
GO

###### Script for adding bonus points to a user
CREATE TABLE #usersForBonusesAddition (
UserId INT
)

DECLARE @bonusPoints INT = <Here goes amount of bonus points>

DECLARE @refundBonusType INT = 3

INSERT INTO #usersForBonusesAddition (UserId)
SELECT Id
FROM Users
WHERE UserName IN ('<Here goes user name>')

UPDATE U
SET TotalBonusPoints += @bonusPoints
FROM Users U
INNER JOIN #usersForBonusesAddition ON #usersForBonusesAddition.UserId = U.Id

INSERT UsersBonusPoints (Value, UserId, OldBonus, DateCreated, Type)
SELECT @bonusPoints, #usersForBonusesAddition.UserId, 0, GETUTCDATE(), @refundBonusType
FROM #usersForBonusesAddition

DROP TABLE #usersForBonusesAddition

######  Script for deletion of a user

DECLARE @userId INT = (SELECT Id FROM Users WHERE UserName = '<Here goes user name>')

CREATE TABLE #userPosOperationIds (
Id INT)

INSERT INTO #userPosOperationIds
SELECT Id FROM PosOperations WHERE UserId = @userId

DELETE FROM CheckItemsAuditHistory
WHERE CheckItemId IN (SELECT Id FROM CheckItems WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds))

DELETE FROM FiscalizationCheckItems
WHERE CheckItemId IN (SELECT Id FROM CheckItems WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds))

UPDATE PosOperationTransactions
SET 
LastBankTransactionInfoId = NULL,
LastFiscalizationInfoId = NULL
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds);

DELETE FROM BankTransactionInfosVersionTwo
WHERE PosOperationTransactionId IN (SELECT Id FROM PosOperationTransactions WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds))

DELETE FROM PosOperationTransactionCheckItems
WHERE CheckItemId IN (SELECT Id FROM CheckItems WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds))

DELETE FROM FiscalizationInfosVersionTwo
WHERE PosOperationTransactionId IN (SELECT Id FROM PosOperationTransactions WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds))

DELETE FROM PosOperationTransactions
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds)

DELETE FROM FiscalizationInfos
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds)

DELETE FROM CheckItems
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds)

DELETE FROM BankTransactionInfos
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds)

DELETE FROM PosDoorsStates
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds)

UPDATE LabeledGoods
SET PosOperationId = NULL
WHERE PosOperationId IN (SELECT Id FROM #userPosOperationIds);

DELETE FROM PosOperations
WHERE UserId = @userId

UPDATE Users
SET ActivePaymentCardId = NULL
WHERE Id = @userId

DELETE FROM PaymentCards
WHERE UserId = @userId

DELETE FROM UsersBonusPoints
WHERE UserId = @userId

DELETE FROM PromotionLogs
WHERE UserId = @userId

DELETE FROM Feedbacks
WHERE UserId = @userId

DELETE FROM UserNotifications
WHERE UserId = @userId

DELETE FROM UserFirebaseTokens
WHERE UserId = @userId

DELETE FROM Users
WHERE Id = @userId

DROP TABLE #userPosOperationIds

##### Инструкция по эмуляции покупки на витрине #####
1. Открываем админку и идем в меню Продажи-Экземпляры. Далее выбираем витрину в которой мы хотим произвести покупку в дропдауне витрин. Важно чтобы в витрине были товары.

2. Идем в Management Studio и выполняем скрипт, который даст нам метки товаров:
DECLARE @PosId INT = 1

DECLARE @Names VARCHAR(8000) 
SELECT @Names = COALESCE(@Names + ', ', '') + '"'+lg.Label+'"'
FROM LabeledGoods lg
WHERE lg.PosId = @PosId
AND PosOperationId IS NULL

SELECT @Names

3. Устанавливаем веб-сокет экстеншн в хром браузер и настраиваем на апи в моем случае url выглядит вот так - ws://localhost:57603/plant. В поле мессадж вставляем вот этот json:
{
  "H": "accountingBalances",
  "M": "synchronize",
  "A": {
    "posId": 1,
    "labels": []
  }
}
массив меток мы получаем из шага №2, просто вставляем его сюда и удаляем пару меток (это как раз будут те товары, которые якобы взял пользователь)

4. Возвращаемся в админку и заходим в витрины, открываем карточку витрины и далее открываем какую-либо дверь, затем закрываем ее (эти манипуляции создадут нам пустую PosOperation)

5. Идем обратно в Management Studio и выполняем скрипт:
DECLARE @PosOperationId INT

SELECT @PosOperationId = MAX(Id) FROM PosOperations
SELECT @PosOperationId

-- set purchase mode 
UPDATE PosOperations SET Mode = 0 WHERE Id = @PosOperationId

-- add user bonuses 
--UPDATE Users SET TotalBonus = 0 WHERE Id = 1

--use discounts
--UPDATE PosOperations SET DateStarted = '2019-07-04 18:21:54.9908205' WHERE Id = @PosOperationId

-- set correct date time after discounts
--UPDATE PosOperations SET DateStarted = '2019-07-04 10:21:54.9908205' WHERE Id = 37408
Все что закомментировано - опционально, для проверки как будут работать скидки и бонусы, главное проставить операции режим покупки

6. Идем в хром и отправляем запрос через веб-сокет клиент.

##### Инструкция по тактической диагностике #####
1. Чтобы тактическая диагностика работала на бете, нужно вставить QR-код работающей витрины на бете.

Для этого заходим в админку беты и смотрим какая витрина подключена, после этого идём в базу данных и достаём витрину:

-- get QrCode
SELECT QrCode FROM PointsOfSale WHERE Id = Id подключенной витрины на бете

Копируем значение и заменяем его в конфигурациях базы

-- set QrCode
DECLARE @PosQrCode NVARCHAR(100) = N'Скопированный ранее QrCode витрины'
UPDATE ConfigurationValues
SET Value = @PosQrCode
WHERE KeyId = 50304

2. Запуск тактической диагностики

Чтобы запустить тактическую диагностику нужно поставить Postman https://www.getpostman.com/

Получение авторизационного токена:
POST запрос на адрес http://api url/connect/token

в Headers указываем:
grant_type: password
username: ваш логин
password: ваш пароль

Чтобы запустить тактическую диагностику на бете, нужно отправить POST запрос с авторизационным токеном, токен указывать во вкладке Authorization -> Type выбираем Bearer Token -> вставляем токен
https://betaapi.nasladdin.club/api/TacticalDiagnostics

##### Инструкция по установке сертификата для Checkonline #####
Чеконлайн присылает нам данные по обновленному сертификату в виде base64 строк PEM стандарт (данные сертификата и приватный ключ). Чтобы это заработало в нашей системе необходимо следующее:  Затем полученный сертификат надо сконвертировать в base64 строку
1. данные сертификата от чеконлайн сохранить в отдельные файлы
2. зайти на сайт онлайн генерации сертификатов и сгенерировать сертификат (из PEM в PFX/PKCS#12), при этом задав какой-либо пароль
3. полученный сертификат надо сконвертировать в base64 строку
4. в классе CheckOnlineConfigurations и в секретах прописать полученную строку в поле CertificateData

##### Инструкция по сборке React проекта#####
Инструкция по сборке лежит в файле ReadmeDeployment в корне проекта Nasladdin.Ui.React

