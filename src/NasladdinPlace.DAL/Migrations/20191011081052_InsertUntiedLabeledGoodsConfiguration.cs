using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertUntiedLabeledGoodsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (600, 0, N'UntiedLabeledGoods', NULL, NULL),
                         (601, 0, N'IdentificationAdminPageBaseUrlFormat', NULL, 600),
                         (602, 3, N'RecheckIntervalInMinutes', NULL, 600),
                         (603, 0, N'DocumentGoodsMovingPageUrlFormat', NULL, 600)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (601, N'https://online.nasladdin.club/PointsOfSale/{0}/LabeledGoods/Identification'),
                         (602, N'120'),
                         (603, N'https://online.nasladdin.club/GoodsMoving/DocumentGoodsMoving/{0}')

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

                    DELETE FROM ConfigurationValues WHERE KeyId IN (601, 602, 603);
                    DELETE FROM ConfigurationKeys WHERE Id IN (600, 601, 602, 603);

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
