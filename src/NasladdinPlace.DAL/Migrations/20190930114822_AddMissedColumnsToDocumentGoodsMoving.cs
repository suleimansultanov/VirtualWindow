using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddMissedColumnsToDocumentGoodsMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DocumentsGoodsMoving",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ErpId",
                table: "DocumentsGoodsMoving",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "DocumentsGoodsMoving",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DocumentsGoodsMoving");

            migrationBuilder.DropColumn(
                name: "ErpId",
                table: "DocumentsGoodsMoving");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "DocumentsGoodsMoving");
        }
    }
}
