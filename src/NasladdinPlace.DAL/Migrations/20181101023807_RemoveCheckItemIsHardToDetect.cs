using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RemoveCheckItemIsHardToDetect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHardToDetect",
                table: "CheckItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHardToDetect",
                table: "CheckItems",
                nullable: false,
                defaultValue: false);
        }
    }
}
