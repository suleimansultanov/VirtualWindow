using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddColumnsToLabeledGoodTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UnexpectedFoundDateTime",
                table: "LabeledGoods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnexpectedLossDateTime",
                table: "LabeledGoods",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnexpectedFoundDateTime",
                table: "LabeledGoods");

            migrationBuilder.DropColumn(
                name: "UnexpectedLossDateTime",
                table: "LabeledGoods");
        }
    }
}
