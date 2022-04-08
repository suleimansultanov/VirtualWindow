using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AlterTablePromotionSettingColumnPromotionTypeAsUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PromotionSettings_PromotionType",
                table: "PromotionSettings",
                column: "PromotionType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromotionSettings_PromotionType",
                table: "PromotionSettings");
        }
    }
}
