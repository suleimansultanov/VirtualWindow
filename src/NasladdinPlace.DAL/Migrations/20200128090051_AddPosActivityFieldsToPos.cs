using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosActivityFieldsToPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PosActivityStatus",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PosActivityStatusChangeDate",
                table: "PointsOfSale",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(@"UPDATE PointsOfSale 
                                    SET PosActivityStatus = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosActivityStatus",
                table: "PointsOfSale");

            migrationBuilder.DropColumn(
                name: "PosActivityStatusChangeDate",
                table: "PointsOfSale");
        }
    }
}
