using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class DropUniqueIndexOnRequestIdInFiscalizationInfosVersionTwoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FiscalizationInfosVersionTwo_RequestId",
                table: "FiscalizationInfosVersionTwo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationInfosVersionTwo_RequestId",
                table: "FiscalizationInfosVersionTwo",
                column: "RequestId",
                unique: true);
        }
    }
}
