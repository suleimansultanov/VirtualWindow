using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AllowPointsOfSaleGoodsPlacingMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO AllowedPosModes (PosId, Mode)
                SELECT Id, 2
                FROM PointsOfSale
                WHERE Id NOT IN (
                  SELECT Id FROM PointsOfSale
                  INNER JOIN AllowedPosModes APM on PointsOfSale.Id = APM.PosId
                  WHERE APM.Mode = 2
                )"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
