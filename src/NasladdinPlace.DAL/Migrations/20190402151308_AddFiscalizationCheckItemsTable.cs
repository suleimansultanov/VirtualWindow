using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddFiscalizationCheckItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FiscalizationInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FiscalizationCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FiscalizationInfoId = table.Column<int>(nullable: false),
                    CheckItemId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalizationCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalizationCheckItems_CheckItems_CheckItemId",
                        column: x => x.CheckItemId,
                        principalTable: "CheckItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FiscalizationCheckItems_FiscalizationInfos_FiscalizationInfoId",
                        column: x => x.FiscalizationInfoId,
                        principalTable: "FiscalizationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationCheckItems_CheckItemId",
                table: "FiscalizationCheckItems",
                column: "CheckItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationCheckItems_FiscalizationInfoId",
                table: "FiscalizationCheckItems",
                column: "FiscalizationInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiscalizationCheckItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FiscalizationInfos");
        }
    }
}
