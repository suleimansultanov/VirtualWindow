using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddRolePermittedAppFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolesPermittedAppFeatures",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    AppFeature = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesPermittedAppFeatures", x => new { x.RoleId, x.AppFeature });
                    table.ForeignKey(
                        name: "FK_RolesPermittedAppFeatures_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermittedAppFeatures_AppFeature",
                table: "RolesPermittedAppFeatures",
                column: "AppFeature");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesPermittedAppFeatures");
        }
    }
}
