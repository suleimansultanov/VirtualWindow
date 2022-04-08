using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddAdditionalColumnsToBonusPointsPosOperationTransactionAndPosOperationTransactionCheckItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "PosOperationTransactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscountAmount",
                table: "PosOperationTransactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CostInBonusPoints",
                table: "PosOperationTransactionCheckItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "PosOperationTransactionCheckItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableBonusPoints",
                table: "BonusPoints",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "PosOperationTransactions");

            migrationBuilder.DropColumn(
                name: "TotalDiscountAmount",
                table: "PosOperationTransactions");

            migrationBuilder.DropColumn(
                name: "CostInBonusPoints",
                table: "PosOperationTransactionCheckItems");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "PosOperationTransactionCheckItems");

            migrationBuilder.DropColumn(
                name: "AvailableBonusPoints",
                table: "BonusPoints");
        }
    }
}
