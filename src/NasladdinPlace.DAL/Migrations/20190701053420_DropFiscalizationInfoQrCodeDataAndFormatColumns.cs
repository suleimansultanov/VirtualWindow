using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class DropFiscalizationInfoQrCodeDataAndFormatColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "FiscalizationInfos");

            migrationBuilder.DropColumn(
                name: "QrCodeFormat",
                table: "FiscalizationInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "FiscalizationInfos",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeFormat",
                table: "FiscalizationInfos",
                maxLength: 10,
                nullable: true);
        }
    }
}
