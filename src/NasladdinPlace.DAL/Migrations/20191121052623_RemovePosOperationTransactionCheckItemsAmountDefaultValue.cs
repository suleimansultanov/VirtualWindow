using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RemovePosOperationTransactionCheckItemsAmountDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PosOperationTransactionCheckItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldDefaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PosOperationTransactionCheckItems",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal));
        }
    }
}
