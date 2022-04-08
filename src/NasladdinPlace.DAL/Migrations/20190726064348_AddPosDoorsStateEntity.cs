using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosDoorsStateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosDoorsStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PosId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosDoorsStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosDoorsStates_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosDoorsStates_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosDoorsStates_PosId",
                table: "PosDoorsStates",
                column: "PosId");

            migrationBuilder.CreateIndex(
                name: "IX_PosDoorsStates_PosOperationId_PosId",
                table: "PosDoorsStates",
                columns: new[] { "PosOperationId", "PosId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosDoorsStates");
        }
    }
}
