using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameUserInPaymentSystemToPaymentCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentCardId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInPaymentSystems_Users_UserId",
                table: "UsersInPaymentSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersInPaymentSystems",
                table: "UsersInPaymentSystems");

            migrationBuilder.RenameTable(
                name: "UsersInPaymentSystems",
                newName: "PaymentCards");

            migrationBuilder.RenameIndex(
                name: "IX_UsersInPaymentSystems_UserId",
                table: "PaymentCards",
                newName: "IX_PaymentCards_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentCards",
                table: "PaymentCards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCards_Users_UserId",
                table: "PaymentCards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PaymentCards_ActivePaymentCardId",
                table: "Users",
                column: "ActivePaymentCardId",
                principalTable: "PaymentCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCards_Users_UserId",
                table: "PaymentCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_PaymentCards_ActivePaymentCardId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentCards",
                table: "PaymentCards");

            migrationBuilder.RenameTable(
                name: "PaymentCards",
                newName: "UsersInPaymentSystems");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentCards_UserId",
                table: "UsersInPaymentSystems",
                newName: "IX_UsersInPaymentSystems_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersInPaymentSystems",
                table: "UsersInPaymentSystems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentCardId",
                table: "Users",
                column: "ActivePaymentCardId",
                principalTable: "UsersInPaymentSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInPaymentSystems_Users_UserId",
                table: "UsersInPaymentSystems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
