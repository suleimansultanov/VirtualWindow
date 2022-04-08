using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameApplicationUserActivePaymentSystemIdToActivePaymentCardId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentSystemId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ActivePaymentSystemId",
                table: "Users",
                newName: "ActivePaymentCardId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ActivePaymentSystemId",
                table: "Users",
                newName: "IX_Users_ActivePaymentCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentCardId",
                table: "Users",
                column: "ActivePaymentCardId",
                principalTable: "UsersInPaymentSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentCardId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ActivePaymentCardId",
                table: "Users",
                newName: "ActivePaymentSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ActivePaymentCardId",
                table: "Users",
                newName: "IX_Users_ActivePaymentSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentSystemId",
                table: "Users",
                column: "ActivePaymentSystemId",
                principalTable: "UsersInPaymentSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
