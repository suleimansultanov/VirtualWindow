using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class DropUseLegacySensorsPositionFieldFromPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseLegacySensorsPosition",
                table: "PointsOfSale");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseLegacySensorsPosition",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: false);
        }
    }
}
