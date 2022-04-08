using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameTableToFiscalizationInfoQrCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FiscalizationInfoQrCode_FiscalizationInfos_Id",
                table: "FiscalizationInfoQrCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FiscalizationInfoQrCode",
                table: "FiscalizationInfoQrCode");

            migrationBuilder.RenameTable(
                name: "FiscalizationInfoQrCode",
                newName: "FiscalizationInfoQrCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FiscalizationInfoQrCodes",
                table: "FiscalizationInfoQrCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FiscalizationInfoQrCodes_FiscalizationInfos_Id",
                table: "FiscalizationInfoQrCodes",
                column: "Id",
                principalTable: "FiscalizationInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FiscalizationInfoQrCodes_FiscalizationInfos_Id",
                table: "FiscalizationInfoQrCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FiscalizationInfoQrCodes",
                table: "FiscalizationInfoQrCodes");

            migrationBuilder.RenameTable(
                name: "FiscalizationInfoQrCodes",
                newName: "FiscalizationInfoQrCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FiscalizationInfoQrCode",
                table: "FiscalizationInfoQrCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FiscalizationInfoQrCode_FiscalizationInfos_Id",
                table: "FiscalizationInfoQrCode",
                column: "Id",
                principalTable: "FiscalizationInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
