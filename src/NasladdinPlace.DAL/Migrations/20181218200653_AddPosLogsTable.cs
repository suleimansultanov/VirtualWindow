using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PosId = table.Column<int>(nullable: false),
                    DateTimeCreated = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: false),
                    FileContent = table.Column<byte[]>(maxLength: 26214400, nullable: false),
                    LogType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosLogs_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosLogs_PosId",
                table: "PosLogs",
                column: "PosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosLogs");
        }
    }
}
