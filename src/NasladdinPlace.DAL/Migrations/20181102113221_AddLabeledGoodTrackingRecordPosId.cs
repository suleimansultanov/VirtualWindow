using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddLabeledGoodTrackingRecordPosId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosId",
                table: "LabeledGoodsTrackingHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LabeledGoodsTrackingHistory_PosId",
                table: "LabeledGoodsTrackingHistory",
                column: "PosId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabeledGoodsTrackingHistory_PointsOfSale_PosId",
                table: "LabeledGoodsTrackingHistory",
                column: "PosId",
                principalTable: "PointsOfSale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabeledGoodsTrackingHistory_PointsOfSale_PosId",
                table: "LabeledGoodsTrackingHistory");

            migrationBuilder.DropIndex(
                name: "IX_LabeledGoodsTrackingHistory_PosId",
                table: "LabeledGoodsTrackingHistory");

            migrationBuilder.DropColumn(
                name: "PosId",
                table: "LabeledGoodsTrackingHistory");
        }
    }
}
