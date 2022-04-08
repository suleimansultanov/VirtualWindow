using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosScreenTemplatesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {         
            migrationBuilder.CreateTable(
                name: "PosScreenTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosScreenTemplates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PosScreenTemplates",
                columns: new[] { "Id", "Name" },
                values: new object[] { 0, "Default" });

            migrationBuilder.AddColumn<int>(
                name: "PosScreenTemplateId",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScreenTemplateAppliedDate",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PointsOfSale_PosScreenTemplateId",
                table: "PointsOfSale",
                column: "PosScreenTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointsOfSale_PosScreenTemplates_PosScreenTemplateId",
                table: "PointsOfSale",
                column: "PosScreenTemplateId",
                principalTable: "PosScreenTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointsOfSale_PosScreenTemplates_PosScreenTemplateId",
                table: "PointsOfSale");

            migrationBuilder.DropTable(
                name: "PosScreenTemplates");

            migrationBuilder.DropIndex(
                name: "IX_PointsOfSale_PosScreenTemplateId",
                table: "PointsOfSale");

            migrationBuilder.DropColumn(
                name: "PosScreenTemplateId",
                table: "PointsOfSale");

            migrationBuilder.DropColumn(
                name: "ScreenTemplateAppliedDate",
                table: "PointsOfSale");
        }
    }
}
