using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameUserBonusesToUserBonusPointsAndAddTwoFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBonuses_Users_UserId",
                table: "UsersBonuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersBonuses",
                table: "UsersBonuses");

            migrationBuilder.RenameTable(
                name: "UsersBonuses",
                newName: "UsersBonusPoints");

            migrationBuilder.RenameIndex(
                name: "IX_UsersBonuses_UserId",
                table: "UsersBonusPoints",
                newName: "IX_UsersBonusPoints_UserId");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableBonusPoints",
                table: "UsersBonusPoints",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PosOperationTransactionId",
                table: "UsersBonusPoints",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersBonusPoints",
                table: "UsersBonusPoints",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersBonusPoints_PosOperationTransactionId",
                table: "UsersBonusPoints",
                column: "PosOperationTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBonusPoints_PosOperationTransactions_PosOperationTransactionId",
                table: "UsersBonusPoints",
                column: "PosOperationTransactionId",
                principalTable: "PosOperationTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBonusPoints_Users_UserId",
                table: "UsersBonusPoints",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersBonusPoints_PosOperationTransactions_PosOperationTransactionId",
                table: "UsersBonusPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersBonusPoints_Users_UserId",
                table: "UsersBonusPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersBonusPoints",
                table: "UsersBonusPoints");

            migrationBuilder.DropIndex(
                name: "IX_UsersBonusPoints_PosOperationTransactionId",
                table: "UsersBonusPoints");

            migrationBuilder.DropColumn(
                name: "AvailableBonusPoints",
                table: "UsersBonusPoints");

            migrationBuilder.DropColumn(
                name: "PosOperationTransactionId",
                table: "UsersBonusPoints");

            migrationBuilder.RenameTable(
                name: "UsersBonusPoints",
                newName: "UsersBonuses");

            migrationBuilder.RenameIndex(
                name: "IX_UsersBonusPoints_UserId",
                table: "UsersBonuses",
                newName: "IX_UsersBonuses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersBonuses",
                table: "UsersBonuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersBonuses_Users_UserId",
                table: "UsersBonuses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
