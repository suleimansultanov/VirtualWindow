using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenamePaymentCardCardTokenToToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardToken",
                table: "PaymentCards",
                newName: "Token");

            migrationBuilder.AddColumn<int>(
                name: "CryptogramSource",
                table: "PaymentCards",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryptogramSource",
                table: "PaymentCards");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "PaymentCards",
                newName: "CardToken");
        }
    }
}
