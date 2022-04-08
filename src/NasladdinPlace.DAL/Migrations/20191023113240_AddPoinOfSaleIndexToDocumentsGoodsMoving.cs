using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPoinOfSaleIndexToDocumentsGoodsMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DocumentsGoodsMoving_PosId",
                table: "DocumentsGoodsMoving",
                column: "PosId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentsGoodsMoving_PointsOfSale_PosId",
                table: "DocumentsGoodsMoving",
                column: "PosId",
                principalTable: "PointsOfSale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentsGoodsMoving_PointsOfSale_PosId",
                table: "DocumentsGoodsMoving");

            migrationBuilder.DropIndex(
                name: "IX_DocumentsGoodsMoving_PosId",
                table: "DocumentsGoodsMoving");
        }
    }
}
