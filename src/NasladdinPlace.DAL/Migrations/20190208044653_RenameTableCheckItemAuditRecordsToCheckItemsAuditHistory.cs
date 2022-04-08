using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameTableCheckItemAuditRecordsToCheckItemsAuditHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckItemAuditRecords_CheckItems_CheckItemId",
                table: "CheckItemAuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckItemAuditRecords_Users_EditorId",
                table: "CheckItemAuditRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckItemAuditRecords",
                table: "CheckItemAuditRecords");

            migrationBuilder.RenameTable(
                name: "CheckItemAuditRecords",
                newName: "CheckItemsAuditHistory");

            migrationBuilder.RenameIndex(
                name: "IX_CheckItemAuditRecords_EditorId",
                table: "CheckItemsAuditHistory",
                newName: "IX_CheckItemsAuditHistory_EditorId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckItemAuditRecords_CheckItemId",
                table: "CheckItemsAuditHistory",
                newName: "IX_CheckItemsAuditHistory_CheckItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckItemsAuditHistory",
                table: "CheckItemsAuditHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckItemsAuditHistory_CheckItems_CheckItemId",
                table: "CheckItemsAuditHistory",
                column: "CheckItemId",
                principalTable: "CheckItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckItemsAuditHistory_Users_EditorId",
                table: "CheckItemsAuditHistory",
                column: "EditorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckItemsAuditHistory_CheckItems_CheckItemId",
                table: "CheckItemsAuditHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckItemsAuditHistory_Users_EditorId",
                table: "CheckItemsAuditHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckItemsAuditHistory",
                table: "CheckItemsAuditHistory");

            migrationBuilder.RenameTable(
                name: "CheckItemsAuditHistory",
                newName: "CheckItemAuditRecords");

            migrationBuilder.RenameIndex(
                name: "IX_CheckItemsAuditHistory_EditorId",
                table: "CheckItemAuditRecords",
                newName: "IX_CheckItemAuditRecords_EditorId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckItemsAuditHistory_CheckItemId",
                table: "CheckItemAuditRecords",
                newName: "IX_CheckItemAuditRecords_CheckItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckItemAuditRecords",
                table: "CheckItemAuditRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckItemAuditRecords_CheckItems_CheckItemId",
                table: "CheckItemAuditRecords",
                column: "CheckItemId",
                principalTable: "CheckItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckItemAuditRecords_Users_EditorId",
                table: "CheckItemAuditRecords",
                column: "EditorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
