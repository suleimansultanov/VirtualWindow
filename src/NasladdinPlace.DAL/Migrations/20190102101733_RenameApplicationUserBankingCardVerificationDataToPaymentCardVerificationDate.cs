using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameApplicationUserBankingCardVerificationDataToPaymentCardVerificationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankingCardVerificationInitiationDate",
                table: "Users",
                newName: "PaymentCardVerificationInitiationDate");

            migrationBuilder.RenameColumn(
                name: "BankingCardVerificationCompletionDate",
                table: "Users",
                newName: "PaymentCardCardVerificationCompletionDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentCardVerificationInitiationDate",
                table: "Users",
                newName: "BankingCardVerificationInitiationDate");

            migrationBuilder.RenameColumn(
                name: "PaymentCardCardVerificationCompletionDate",
                table: "Users",
                newName: "BankingCardVerificationCompletionDate");
        }
    }
}
