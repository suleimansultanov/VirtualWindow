using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddProteinsFatsCarbohydratesCalories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProteinsFatsCarbohydratesCalories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProteinsInGrams = table.Column<int>(nullable: false),
                    FatsInGrams = table.Column<int>(nullable: false),
                    CarbohydratesInGrams = table.Column<int>(nullable: false),
                    CaloriesInKcal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProteinsFatsCarbohydratesCalories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProteinsFatsCarbohydratesCalories_Goods_Id",
                        column: x => x.Id,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProteinsFatsCarbohydratesCalories");
        }
    }
}
