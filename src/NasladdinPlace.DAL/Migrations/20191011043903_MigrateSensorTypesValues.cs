using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class MigrateSensorTypesValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    Update PointsOfSale
                    Set SensorControllerType = 0
                    Where UseLegacySensorsPosition = 0
                    Update PointsOfSale
                    Set SensorControllerType = 1 
                    Where UseLegacySensorsPosition = 1

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
