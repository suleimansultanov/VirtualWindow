using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameLabeledGoodUnexpectedlyFoundAndLostColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnexpectedlyLostDateTime",
                table: "LabeledGoods",
                newName: "LostDateTime");

            migrationBuilder.RenameColumn(
                name: "UnexpectedlyFoundDateTime",
                table: "LabeledGoods",
                newName: "FoundDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LostDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedlyLostDateTime");

            migrationBuilder.RenameColumn(
                name: "FoundDateTime",
                table: "LabeledGoods",
                newName: "UnexpectedlyFoundDateTime");
        }
    }
}
