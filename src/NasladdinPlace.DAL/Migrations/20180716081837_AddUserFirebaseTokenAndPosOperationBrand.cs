using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddUserFirebaseTokenAndPosOperationBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirebaseToken",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Brand",
                table: "PosOperations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserFirebaseTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<int>(nullable: false),
                    Token = table.Column<string>(maxLength: 255, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFirebaseTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFirebaseTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFirebaseTokens_UserId_Brand",
                table: "UserFirebaseTokens",
                columns: new[] { "UserId", "Brand" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFirebaseTokens");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "PosOperations");

            migrationBuilder.AddColumn<string>(
                name: "FirebaseToken",
                table: "Users",
                maxLength: 255,
                nullable: true);
        }
    }
}
