using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateEntitiesForShoppingSystemBasedOnCheckItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "PaybackId",
                table: "BankTransactionInfos");

            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "BankTransactionInfos");

            migrationBuilder.RenameColumn(
                name: "RefundReason",
                table: "BankTransactionInfos",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "PaymentAmount",
                table: "BankTransactionInfos",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "FirebaseToken",
                table: "Users",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePaid",
                table: "PosOperations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmountViaBonuses",
                table: "PosOperations",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PosOperations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "BankTransactionInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "BankTransactionInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LabeledGoodId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    IsHardToDetect = table.Column<bool>(nullable: false),
                    GoodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckItems_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckItems_Goods_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckItems_LabeledGoods_LabeledGoodId",
                        column: x => x.LabeledGoodId,
                        principalTable: "LabeledGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckItems_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckItems_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItems_CurrencyId",
                table: "CheckItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItems_GoodId",
                table: "CheckItems",
                column: "GoodId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItems_LabeledGoodId",
                table: "CheckItems",
                column: "LabeledGoodId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItems_PosId",
                table: "CheckItems",
                column: "PosId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckItems_PosOperationId_PosId",
                table: "CheckItems",
                columns: new[] { "PosOperationId", "PosId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.DropTable(
                name: "CheckItems");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                table: "OpenIddictTokens");

            migrationBuilder.DropColumn(
                name: "FirebaseToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DatePaid",
                table: "PosOperations");

            migrationBuilder.DropColumn(
                name: "PaymentAmountViaBonuses",
                table: "PosOperations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PosOperations");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "BankTransactionInfos");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "BankTransactionInfos");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "BankTransactionInfos",
                newName: "RefundReason");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "BankTransactionInfos",
                newName: "PaymentAmount");

            migrationBuilder.AddColumn<int>(
                name: "PaybackId",
                table: "BankTransactionInfos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundDate",
                table: "BankTransactionInfos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);
        }
    }
}
