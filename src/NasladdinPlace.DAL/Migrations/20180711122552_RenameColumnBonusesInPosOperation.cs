using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameColumnBonusesInPosOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentAmountViaBonuses",
                table: "PosOperations",
                newName: "BonusAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BonusAmount",
                table: "PosOperations",
                newName: "PaymentAmountViaBonuses");
        }
    }
}
