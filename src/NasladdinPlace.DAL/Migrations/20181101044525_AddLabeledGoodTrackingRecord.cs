using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddLabeledGoodTrackingRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabeledGoodsTrackingHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LabeledGoodId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabeledGoodsTrackingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabeledGoodsTrackingHistory_LabeledGoods_LabeledGoodId",
                        column: x => x.LabeledGoodId,
                        principalTable: "LabeledGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabeledGoodsTrackingHistory_LabeledGoodId",
                table: "LabeledGoodsTrackingHistory",
                column: "LabeledGoodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabeledGoodsTrackingHistory");
        }
    }
}
