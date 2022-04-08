using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddedTablesForACL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFeatureItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PermissionCategory = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFeatureItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointsOfSaleToRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsOfSaleToRoles", x => new { x.RoleId, x.PosId });
                    table.ForeignKey(
                        name: "FK_PointsOfSaleToRoles_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointsOfSaleToRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppFeatureItemsToRoles",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    AppFeatureItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFeatureItemsToRoles", x => new { x.RoleId, x.AppFeatureItemId });
                    table.ForeignKey(
                        name: "FK_AppFeatureItemsToRoles_AppFeatureItems_AppFeatureItemId",
                        column: x => x.AppFeatureItemId,
                        principalTable: "AppFeatureItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppFeatureItemsToRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFeatureItemsToRoles_AppFeatureItemId",
                table: "AppFeatureItemsToRoles",
                column: "AppFeatureItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsOfSaleToRoles_PosId",
                table: "PointsOfSaleToRoles",
                column: "PosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFeatureItemsToRoles");

            migrationBuilder.DropTable(
                name: "PointsOfSaleToRoles");

            migrationBuilder.DropTable(
                name: "AppFeatureItems");
        }
    }
}
