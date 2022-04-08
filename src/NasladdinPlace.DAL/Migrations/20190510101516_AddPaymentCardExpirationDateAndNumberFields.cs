using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPaymentCardExpirationDateAndNumberFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "PaymentCards",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstSixDigits",
                table: "PaymentCards",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastFourDigits",
                table: "PaymentCards",
                maxLength: 4,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "PaymentCards");

            migrationBuilder.DropColumn(
                name: "FirstSixDigits",
                table: "PaymentCards");

            migrationBuilder.DropColumn(
                name: "LastFourDigits",
                table: "PaymentCards");
        }
    }
}
