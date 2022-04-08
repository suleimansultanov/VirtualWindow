using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenamePosOperationApplicationUserToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosOperations_Users_ApplicationUserId",
                table: "PosOperations");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "PosOperations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PosOperations_ApplicationUserId",
                table: "PosOperations",
                newName: "IX_PosOperations_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperations_Users_UserId",
                table: "PosOperations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PosOperations_Users_UserId",
                table: "PosOperations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PosOperations",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PosOperations_UserId",
                table: "PosOperations",
                newName: "IX_PosOperations_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperations_Users_ApplicationUserId",
                table: "PosOperations",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
