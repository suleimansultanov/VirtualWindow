using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddFiscalizationInfoQrCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FiscalizationInfoQrCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    QrCodeData = table.Column<string>(maxLength: 100000, nullable: false),
                    QrCodeFormat = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalizationInfoQrCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalizationInfoQrCode_FiscalizationInfos_Id",
                        column: x => x.Id,
                        principalTable: "FiscalizationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiscalizationInfoQrCode");
        }
    }
}
