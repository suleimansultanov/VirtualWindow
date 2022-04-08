using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddOfdCheckUrlAndUniqueIndexRequestIdAndAlterRequestIdVersionFiscalizationInfosVersionTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestId",
                table: "FiscalizationInfosVersionTwo",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "OfdCheckUrl",
                table: "FiscalizationInfosVersionTwo",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationInfosVersionTwo_RequestId_RequestDateTime",
                table: "FiscalizationInfosVersionTwo",
                columns: new[] { "RequestId", "RequestDateTime" },
                unique: true,
                filter: "[RequestId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FiscalizationInfosVersionTwo_RequestId_RequestDateTime",
                table: "FiscalizationInfosVersionTwo");

            migrationBuilder.DropColumn(
                name: "OfdCheckUrl",
                table: "FiscalizationInfosVersionTwo");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "FiscalizationInfosVersionTwo",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
