using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class DropGoodHistoryRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodHistoryRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodHistoryRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: false),
                    LabeledGoodId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodHistoryRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodHistoryRecords_LabeledGoods_LabeledGoodId",
                        column: x => x.LabeledGoodId,
                        principalTable: "LabeledGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodHistoryRecords_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodHistoryRecords_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodHistoryRecords_LabeledGoodId",
                table: "GoodHistoryRecords",
                column: "LabeledGoodId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodHistoryRecords_PosId",
                table: "GoodHistoryRecords",
                column: "PosId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodHistoryRecords_PosOperationId_PosId",
                table: "GoodHistoryRecords",
                columns: new[] { "PosOperationId", "PosId" });
        }
    }
}
