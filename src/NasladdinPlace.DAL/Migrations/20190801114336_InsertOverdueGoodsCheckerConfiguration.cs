using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertOverdueGoodsCheckerConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (20400, 0, N'OverdueGoodsChecker', NULL, NULL),
                         (20401, 1, N'OverdueGoodsAgentUpdateInterval', NULL, 20400),
                         (20402, 0, N'OverdueGoodsAdminPageBaseUrl', NULL, 20400)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (20401, N'1:00:00:00'),
                         (20402, N'/LabeledGoods/GetLabeledGoodsGrouppedByGood?posId={0}&type={1}')

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
