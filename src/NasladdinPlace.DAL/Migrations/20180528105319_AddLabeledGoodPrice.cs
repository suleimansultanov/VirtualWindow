using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddLabeledGoodPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodPricesInPointsOfSale");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "LabeledGoods",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "LabeledGoods",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabeledGoods_CurrencyId",
                table: "LabeledGoods",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabeledGoods_Currencies_CurrencyId",
                table: "LabeledGoods",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabeledGoods_Currencies_CurrencyId",
                table: "LabeledGoods");

            migrationBuilder.DropIndex(
                name: "IX_LabeledGoods_CurrencyId",
                table: "LabeledGoods");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "LabeledGoods");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "LabeledGoods");

            migrationBuilder.CreateTable(
                name: "GoodPricesInPointsOfSale",
                columns: table => new
                {
                    PosId = table.Column<int>(nullable: false),
                    GoodId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodPricesInPointsOfSale", x => new { x.PosId, x.GoodId });
                    table.ForeignKey(
                        name: "FK_GoodPricesInPointsOfSale_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodPricesInPointsOfSale_Goods_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodPricesInPointsOfSale_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodPricesInPointsOfSale_CurrencyId",
                table: "GoodPricesInPointsOfSale",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodPricesInPointsOfSale_GoodId",
                table: "GoodPricesInPointsOfSale",
                column: "GoodId");
        }
    }
}
