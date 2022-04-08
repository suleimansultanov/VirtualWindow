using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameApplicationUserPaymentCardCardVerificationDataToPaymentCardVerificationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentCardCardVerificationCompletionDate",
                table: "Users",
                newName: "PaymentCardVerificationCompletionDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentCardVerificationCompletionDate",
                table: "Users",
                newName: "PaymentCardCardVerificationCompletionDate");
        }
    }
}
