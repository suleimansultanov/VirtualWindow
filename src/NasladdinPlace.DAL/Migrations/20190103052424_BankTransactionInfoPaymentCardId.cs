using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class BankTransactionInfoPaymentCardId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentCardId",
                table: "BankTransactionInfos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactionInfos_PaymentCardId",
                table: "BankTransactionInfos",
                column: "PaymentCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactionInfos_PaymentCards_PaymentCardId",
                table: "BankTransactionInfos",
                column: "PaymentCardId",
                principalTable: "PaymentCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactionInfos_PaymentCards_PaymentCardId",
                table: "BankTransactionInfos");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactionInfos_PaymentCardId",
                table: "BankTransactionInfos");

            migrationBuilder.DropColumn(
                name: "PaymentCardId",
                table: "BankTransactionInfos");
        }
    }
}
