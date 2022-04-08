using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class DropFiscalizationQrCodeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiscalizationInfoQrCodes");

            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "FiscalizationInfosVersionTwo");

            migrationBuilder.DropColumn(
                name: "QrCodeFormat",
                table: "FiscalizationInfosVersionTwo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "FiscalizationInfosVersionTwo",
                maxLength: 100000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeFormat",
                table: "FiscalizationInfosVersionTwo",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FiscalizationInfoQrCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    QrCodeData = table.Column<string>(maxLength: 100000, nullable: false),
                    QrCodeFormat = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalizationInfoQrCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalizationInfoQrCodes_FiscalizationInfos_Id",
                        column: x => x.Id,
                        principalTable: "FiscalizationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }
    }
}
