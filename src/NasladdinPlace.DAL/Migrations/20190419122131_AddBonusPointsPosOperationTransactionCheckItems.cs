using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddBonusPointsPosOperationTransactionCheckItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PosOperationTransactions_LastBankTransactionInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PosOperationTransactions_LastFiscalizationInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "LastFiscalizationInfoId",
                table: "PosOperationTransactions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "FiscalizationPaidDate",
                table: "PosOperationTransactions",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "BankTransactionPaidDate",
                table: "PosOperationTransactions",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.CreateTable(
                name: "BonusPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    PosOperationTransactionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusPoints_PosOperationTransactions_PosOperationTransactionId",
                        column: x => x.PosOperationTransactionId,
                        principalTable: "PosOperationTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BonusPoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PosOperationTransactionCheckItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CheckItemId = table.Column<int>(nullable: false),
                    PosOperationTransactionId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosOperationTransactionCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosOperationTransactionCheckItems_CheckItems_CheckItemId",
                        column: x => x.CheckItemId,
                        principalTable: "CheckItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PosOperationTransactionCheckItems_PosOperationTransactions_PosOperationTransactionId",
                        column: x => x.PosOperationTransactionId,
                        principalTable: "PosOperationTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                column: "LastBankTransactionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_LastFiscalizationInfoId",
                table: "PosOperationTransactions",
                column: "LastFiscalizationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusPoints_PosOperationTransactionId",
                table: "BonusPoints",
                column: "PosOperationTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BonusPoints_UserId",
                table: "BonusPoints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactionCheckItems_CheckItemId",
                table: "PosOperationTransactionCheckItems",
                column: "CheckItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactionCheckItems_PosOperationTransactionId",
                table: "PosOperationTransactionCheckItems",
                column: "PosOperationTransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusPoints");

            migrationBuilder.DropTable(
                name: "PosOperationTransactionCheckItems");

            migrationBuilder.DropIndex(
                name: "IX_PosOperationTransactions_LastBankTransactionInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PosOperationTransactions_LastFiscalizationInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "LastFiscalizationInfoId",
                table: "PosOperationTransactions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FiscalizationPaidDate",
                table: "PosOperationTransactions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BankTransactionPaidDate",
                table: "PosOperationTransactions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                column: "LastBankTransactionInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_LastFiscalizationInfoId",
                table: "PosOperationTransactions",
                column: "LastFiscalizationInfoId",
                unique: true);
        }
    }
}
