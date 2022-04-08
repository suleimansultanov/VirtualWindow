using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddRequiredScreenHeightAndWidthToPointsOfSaleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredScreenResolutionHeight",
                table: "PointsOfSale",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredScreenResolutionWidth",
                table: "PointsOfSale",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredScreenResolutionHeight",
                table: "PointsOfSale");

            migrationBuilder.DropColumn(
                name: "RequiredScreenResolutionWidth",
                table: "PointsOfSale");
        }
    }
}
