using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameBankTransactionInfosVersionTwoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransacitonInfosVersionTwo_PaymentCards_PaymentCardId",
                table: "BankTransacitonInfosVersionTwo");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransacitonInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "BankTransacitonInfosVersionTwo");

            migrationBuilder.DropForeignKey(
                name: "FK_PosOperationTransactions_BankTransacitonInfosVersionTwo_LastBankTransactionInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankTransacitonInfosVersionTwo",
                table: "BankTransacitonInfosVersionTwo");

            migrationBuilder.RenameTable(
                name: "BankTransacitonInfosVersionTwo",
                newName: "BankTransactionInfosVersionTwo");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransacitonInfosVersionTwo_PosOperationTransactionId",
                table: "BankTransactionInfosVersionTwo",
                newName: "IX_BankTransactionInfosVersionTwo_PosOperationTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransacitonInfosVersionTwo_PaymentCardId",
                table: "BankTransactionInfosVersionTwo",
                newName: "IX_BankTransactionInfosVersionTwo_PaymentCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankTransactionInfosVersionTwo",
                table: "BankTransactionInfosVersionTwo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PosOperationTransactions_PosId",
                table: "PosOperationTransactions",
                column: "PosId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactionInfosVersionTwo_PaymentCards_PaymentCardId",
                table: "BankTransactionInfosVersionTwo",
                column: "PaymentCardId",
                principalTable: "PaymentCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactionInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "BankTransactionInfosVersionTwo",
                column: "PosOperationTransactionId",
                principalTable: "PosOperationTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperationTransactions_BankTransactionInfosVersionTwo_LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                column: "LastBankTransactionInfoId",
                principalTable: "BankTransactionInfosVersionTwo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperationTransactions_PointsOfSale_PosId",
                table: "PosOperationTransactions",
                column: "PosId",
                principalTable: "PointsOfSale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactionInfosVersionTwo_PaymentCards_PaymentCardId",
                table: "BankTransactionInfosVersionTwo");

            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactionInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "BankTransactionInfosVersionTwo");

            migrationBuilder.DropForeignKey(
                name: "FK_PosOperationTransactions_BankTransactionInfosVersionTwo_LastBankTransactionInfoId",
                table: "PosOperationTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PosOperationTransactions_PointsOfSale_PosId",
                table: "PosOperationTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PosOperationTransactions_PosId",
                table: "PosOperationTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankTransactionInfosVersionTwo",
                table: "BankTransactionInfosVersionTwo");

            migrationBuilder.RenameTable(
                name: "BankTransactionInfosVersionTwo",
                newName: "BankTransacitonInfosVersionTwo");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransactionInfosVersionTwo_PosOperationTransactionId",
                table: "BankTransacitonInfosVersionTwo",
                newName: "IX_BankTransacitonInfosVersionTwo_PosOperationTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_BankTransactionInfosVersionTwo_PaymentCardId",
                table: "BankTransacitonInfosVersionTwo",
                newName: "IX_BankTransacitonInfosVersionTwo_PaymentCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankTransacitonInfosVersionTwo",
                table: "BankTransacitonInfosVersionTwo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransacitonInfosVersionTwo_PaymentCards_PaymentCardId",
                table: "BankTransacitonInfosVersionTwo",
                column: "PaymentCardId",
                principalTable: "PaymentCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransacitonInfosVersionTwo_PosOperationTransactions_PosOperationTransactionId",
                table: "BankTransacitonInfosVersionTwo",
                column: "PosOperationTransactionId",
                principalTable: "PosOperationTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PosOperationTransactions_BankTransacitonInfosVersionTwo_LastBankTransactionInfoId",
                table: "PosOperationTransactions",
                column: "LastBankTransactionInfoId",
                principalTable: "BankTransacitonInfosVersionTwo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
