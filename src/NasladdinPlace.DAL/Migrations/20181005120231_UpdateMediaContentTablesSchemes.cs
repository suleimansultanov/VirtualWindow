using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateMediaContentTablesSchemes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaContentToPlatforms");

            migrationBuilder.DropColumn(
                name: "PosScreenImageType",
                table: "PosMediaContents");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "MediaContents");

            migrationBuilder.RenameColumn(
                name: "CreatingDateTime",
                table: "PosMediaContents",
                newName: "DateTimeCreated");

            migrationBuilder.AddColumn<int>(
                name: "PosScreenType",
                table: "PosMediaContents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaContents",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "FileContent",
                table: "MediaContents",
                maxLength: 26214400,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MediaContentToPosPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MediaContentId = table.Column<int>(nullable: false),
                    PosScreenType = table.Column<int>(nullable: false),
                    DateTimeCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaContentToPosPlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaContentToPosPlatforms_MediaContents_MediaContentId",
                        column: x => x.MediaContentId,
                        principalTable: "MediaContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaContentToPosPlatforms_MediaContentId",
                table: "MediaContentToPosPlatforms",
                column: "MediaContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaContentToPosPlatforms");

            migrationBuilder.DropColumn(
                name: "PosScreenType",
                table: "PosMediaContents");

            migrationBuilder.RenameColumn(
                name: "DateTimeCreated",
                table: "PosMediaContents",
                newName: "CreatingDateTime");

            migrationBuilder.AddColumn<int>(
                name: "PosScreenImageType",
                table: "PosMediaContents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaContents",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<byte[]>(
                name: "FileContent",
                table: "MediaContents",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldMaxLength: 26214400);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "MediaContents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MediaContentToPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatingDateTime = table.Column<DateTime>(nullable: false),
                    MediaContentId = table.Column<int>(nullable: false),
                    PlatformType = table.Column<int>(nullable: false),
                    PosScreenImageType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaContentToPlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaContentToPlatforms_MediaContents_MediaContentId",
                        column: x => x.MediaContentId,
                        principalTable: "MediaContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaContentToPlatforms_MediaContentId",
                table: "MediaContentToPlatforms",
                column: "MediaContentId");
        }
    }
}
