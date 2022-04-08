using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameFieldsInPosOperationTransactionAndFiscalizationInfoVersionTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FiscalizationPaidDate",
                table: "PosOperationTransactions",
                newName: "FiscalizationDate");

            migrationBuilder.RenameColumn(
                name: "DateTimeResponse",
                table: "FiscalizationInfosVersionTwo",
                newName: "ResponseDateTime");

            migrationBuilder.RenameColumn(
                name: "DateTimeRequest",
                table: "FiscalizationInfosVersionTwo",
                newName: "RequestDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FiscalizationDate",
                table: "PosOperationTransactions",
                newName: "FiscalizationPaidDate");

            migrationBuilder.RenameColumn(
                name: "ResponseDateTime",
                table: "FiscalizationInfosVersionTwo",
                newName: "DateTimeResponse");

            migrationBuilder.RenameColumn(
                name: "RequestDateTime",
                table: "FiscalizationInfosVersionTwo",
                newName: "DateTimeRequest");
        }
    }
}
