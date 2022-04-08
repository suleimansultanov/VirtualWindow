using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddedAccurateLocationAndIsRestrictedAccessPropertiesInPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccurateLocation",
                table: "PointsOfSale",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRestrictedAccess",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccurateLocation",
                table: "PointsOfSale");

            migrationBuilder.DropColumn(
                name: "IsRestrictedAccess",
                table: "PointsOfSale");
        }
    }
}
