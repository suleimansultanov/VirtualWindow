using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameCheckItemColumnIsCreatedByAdminToIsModifiedByAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCreatedByAdmin",
                table: "CheckItems",
                newName: "IsModifiedByAdmin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsModifiedByAdmin",
                table: "CheckItems",
                newName: "IsCreatedByAdmin");
        }
    }
}
