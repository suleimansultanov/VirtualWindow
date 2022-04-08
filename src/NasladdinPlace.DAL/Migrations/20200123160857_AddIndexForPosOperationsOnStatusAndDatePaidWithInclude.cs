using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexForPosOperationsOnStatusAndDatePaidWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX 
					[IX_PosOperation_Status_DatePaid_Include] 
				ON [dbo].[PosOperations] 
					([Status], [DatePaid]) 
				INCLUDE (
					[AuditCompletionDateTime],
					[AuditRequestDateTime], 
					[BonusAmount], 
					[Brand], 
					[CompletionInitiationDate], 
					[CorrectnessStatus], 
					[DateCompleted], 
					[DateStarted], 
					[Mode], 
					[UserId]
					) WITH (
					PAD_INDEX = ON,
					FILLFACTOR = 90,
					ONLINE = OFF )" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropIndex( "IX_PosOperation_Status_DatePaid_Include", "PosOperations" );
		}
    }
}
