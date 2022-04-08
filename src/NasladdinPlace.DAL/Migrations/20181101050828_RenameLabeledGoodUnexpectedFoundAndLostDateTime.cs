using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameLabeledGoodUnexpectedFoundAndLostDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnexpectedLossDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedlyLostDateTime");

            migrationBuilder.RenameColumn(
                name: "UnexpectedFoundDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedlyFoundDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnexpectedlyLostDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedLossDateTime");

            migrationBuilder.RenameColumn(
                name: "UnexpectedlyFoundDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedFoundDateTime");
        }
    }
}
