using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddUserInPaymentSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivePaymentSystemId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsersInPaymentSystems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardToken = table.Column<string>(maxLength: 255, nullable: false),
                    PaymentSystem = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInPaymentSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersInPaymentSystems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActivePaymentSystemId",
                table: "Users",
                column: "ActivePaymentSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInPaymentSystems_UserId",
                table: "UsersInPaymentSystems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentSystemId",
                table: "Users",
                column: "ActivePaymentSystemId",
                principalTable: "UsersInPaymentSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersInPaymentSystems_ActivePaymentSystemId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UsersInPaymentSystems");

            migrationBuilder.DropIndex(
                name: "IX_Users_ActivePaymentSystemId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActivePaymentSystemId",
                table: "Users");
        }
    }
}
