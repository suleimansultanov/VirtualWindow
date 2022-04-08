using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InstertPosStateHistoricalDataDeletingConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (10107, 3, N'PosStateDataLifeTimePeriodInDays', NULL, 10100),
						 (10108, 1, N'DeletingObsoleteHistoricalDataStartTime', NULL, 10100)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (10107, N'10'),
						 (10108, N'01:00:00')

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
