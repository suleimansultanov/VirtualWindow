using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class ChangePosOperationDateSentForVerificationToCompletionInitiationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateSentForVerification",
                table: "PosOperations",
                newName: "CompletionInitiationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletionInitiationDate",
                table: "PosOperations",
                newName: "DateSentForVerification");
        }
    }
}
