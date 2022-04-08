using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddGoodCategoriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "GoodCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 0, "Default" });

            migrationBuilder.AddColumn<int>(
                name: "GoodCategoryId",
                table: "Goods",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_GoodCategoryId",
                table: "Goods",
                column: "GoodCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goods_GoodCategories_GoodCategoryId",
                table: "Goods",
                column: "GoodCategoryId",
                principalTable: "GoodCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goods_GoodCategories_GoodCategoryId",
                table: "Goods");

            migrationBuilder.DropTable(
                name: "GoodCategories");

            migrationBuilder.DropIndex(
                name: "IX_Goods_GoodCategoryId",
                table: "Goods");

            migrationBuilder.DropColumn(
                name: "GoodCategoryId",
                table: "Goods");
        }
    }
}
