using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosOperationTransactionsBankTransactionInfosVersionTwoFiscalizationInfosVersionTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosOperationTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PosOperationId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false),
                    LastBankTransactionInfoId = table.Column<int>(nullable: false),
                    LastFiscalizationInfoId = table.Column<int>(nullable: false),
                    BonusAmount = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    MoneyAmount = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    FiscalizationAmount = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    BankTransactionPaidDate = table.Column<DateTime>(nullable: false),
                    FiscalizationPaidDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosOperationTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosOperationTransactions_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankTransacitonInfosVersionTwo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankTransactionId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 3000, nullable: true),
                    PaymentCardId = table.Column<int>(nullable: true),
                    PosOperationTransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransacitonInfosVersionTwo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransacitonInfosVersionTwo_PaymentCards_PaymentCardId",
                        column: x => x.PaymentCardId,
                        principalTable: "PaymentCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankTransacitonInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                        column: x => x.PosOperationTransactionId,
                        principalTable: "PosOperationTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalizationInfosVersionTwo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    DateTimeRequest = table.Column<DateTime>(nullable: false),
                    DateTimeResponse = table.Column<DateTime>(nullable: true),
                    ErrorInfo = table.Column<string>(maxLength: 2000, nullable: true),
                    DocumentInfo = table.Column<string>(maxLength: 2000, nullable: true),
                    QrCodeData = table.Column<string>(maxLength: 100000, nullable: true),
                    QrCodeFormat = table.Column<string>(maxLength: 10, nullable: true),
                    FiscalizationNumber = table.Column<string>(maxLength: 50, nullable: true),
                    FiscalizationSerial = table.Column<string>(maxLength: 50, nullable: true),
                    FiscalizationSign = table.Column<string>(maxLength: 50, nullable: true),
                    DocumentDateTime = table.Column<DateTime>(nullable: true),
                    CorrectionAmount = table.Column<decimal>(nullable: true),
                    TotalFiscalizationAmount = table.Column<decimal>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    PosOperationTransactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalizationInfosVersionTwo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalizationInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                        column: x => x.PosOperationTransactionId,
                        principalTable: "PosOperationTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransacitonInfosVersionTwo_PaymentCardId",
                table: "BankTransacitonInfosVersionTwo",
                column: "PaymentCardId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransacitonInfosVersionTwo_PosOperationTransactionId",
                table: "BankTransacitonInfosVersionTwo",
                column: "PosOperationTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationInfosVersionTwo_PosOperationTransactionId",
                table: "FiscalizationInfosVersionTwo",
                column: "PosOperationTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationInfosVersionTwo_RequestId",
                table: "FiscalizationInfosVersionTwo",
                column: "RequestId",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_PosOperationId_PosId",
                table: "PosOperationTransactions",
                columns: new[] { "PosOperationId", "PosId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperationTransactions_BankTransacitonInfosVersionTwo_LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                column: "LastBankTransactionInfoId",
                principalTable: "BankTransacitonInfosVersionTwo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperationTransactions_FiscalizationInfosVersionTwo_LastFiscalizationInfoId",
                table: "PosOperationTransactions",
                column: "LastFiscalizationInfoId",
                principalTable: "FiscalizationInfosVersionTwo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransacitonInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "BankTransacitonInfosVersionTwo");

            migrationBuilder.DropForeignKey(
                name: "FK_FiscalizationInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "FiscalizationInfosVersionTwo");

            migrationBuilder.DropTable(
                name: "PosOperationTransactions");

            migrationBuilder.DropTable(
                name: "BankTransacitonInfosVersionTwo");

            migrationBuilder.DropTable(
                name: "FiscalizationInfosVersionTwo");
        }
    }
}
