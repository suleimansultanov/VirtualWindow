using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexForPosOperationWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@" 
                CREATE NONCLUSTERED INDEX [IX_PosOperation_PosId_Include] 
                ON [dbo].[PosOperations] ([PosId]) 
                INCLUDE ([UserId], [AuditCompletionDateTime], [AuditRequestDateTime],
                    [BonusAmount], [Brand], [DateCompleted],    
                    [DatePaid], [DateSentForVerification], [DateStarted], [Mode], [Status])");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_PosOperation_PosId_Include");
        }
    }
}
