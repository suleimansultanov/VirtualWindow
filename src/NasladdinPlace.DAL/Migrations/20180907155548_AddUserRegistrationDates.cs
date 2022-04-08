using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddUserRegistrationDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BankingCardVerificationCompletionDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BankingCardVerificationInitiationDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationCompletionDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationInitiationDate",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankingCardVerificationCompletionDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankingCardVerificationInitiationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationCompletionDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationInitiationDate",
                table: "Users");
        }
    }
}
