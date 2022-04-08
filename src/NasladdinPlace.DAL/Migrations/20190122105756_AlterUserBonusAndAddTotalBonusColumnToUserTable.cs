using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AlterUserBonusAndAddTotalBonusColumnToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bonus",
                table: "UsersBonuses",
                newName: "Value");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "UsersBonuses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "UsersBonuses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBonus",
                table: "Users",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "UsersBonuses");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UsersBonuses");

            migrationBuilder.DropColumn(
                name: "TotalBonus",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "UsersBonuses",
                newName: "Bonus");
        }
    }
}
