using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddCheckItemAuditRecordTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckItemAuditRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CheckItemId = table.Column<int>(nullable: false),
                    OldStatus = table.Column<int>(nullable: false),
                    NewStatus = table.Column<int>(nullable: false),
                    EditorId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckItemAuditRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckItemAuditRecords_CheckItems_CheckItemId",
                        column: x => x.CheckItemId,
                        principalTable: "CheckItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckItemAuditRecords_Users_EditorId",
                        column: x => x.EditorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckItemAuditRecords_CheckItemId",
                table: "CheckItemAuditRecords",
                column: "CheckItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItemAuditRecords_EditorId",
                table: "CheckItemAuditRecords",
                column: "EditorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckItemAuditRecords");
        }
    }
}
