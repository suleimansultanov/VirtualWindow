using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class ChangedPFCCValuesFromIntToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ProteinsInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "FatsInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "CarbohydratesInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "CaloriesInKcal",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProteinsInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "FatsInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "CarbohydratesInGrams",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "CaloriesInKcal",
                table: "ProteinsFatsCarbohydratesCalories",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
