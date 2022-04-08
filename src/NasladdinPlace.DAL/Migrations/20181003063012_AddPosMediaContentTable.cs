using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosMediaContentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaContentToPlatforms",
                table: "MediaContentToPlatforms");

            migrationBuilder.AddColumn<int>(
                name: "PosScreenImageType",
                table: "MediaContentToPlatforms",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaContentToPlatforms",
                table: "MediaContentToPlatforms",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PosMediaContents",
                columns: table => new
                {
                    PosId = table.Column<int>(nullable: false),
                    MediaContentId = table.Column<int>(nullable: false),
                    CreatingDateTime = table.Column<DateTime>(nullable: false),
                    PosScreenImageType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosMediaContents", x => new { x.PosId, x.MediaContentId });
                    table.ForeignKey(
                        name: "FK_PosMediaContents_MediaContents_MediaContentId",
                        column: x => x.MediaContentId,
                        principalTable: "MediaContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosMediaContents_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosMediaContents_MediaContentId",
                table: "PosMediaContents",
                column: "MediaContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosMediaContents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaContentToPlatforms",
                table: "MediaContentToPlatforms");

            migrationBuilder.DropColumn(
                name: "PosScreenImageType",
                table: "MediaContentToPlatforms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaContentToPlatforms",
                table: "MediaContentToPlatforms",
                columns: new[] { "Id", "MediaContentId" });
        }
    }
}
