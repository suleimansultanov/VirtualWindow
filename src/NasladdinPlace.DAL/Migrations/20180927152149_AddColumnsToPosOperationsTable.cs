using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddColumnsToPosOperationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuditRequestDateTime",
                table: "PosOperations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditResponseDateTime",
                table: "PosOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditRequestDateTime",
                table: "PosOperations");

            migrationBuilder.DropColumn(
                name: "AuditResponseDateTime",
                table: "PosOperations");
        }
    }
}
