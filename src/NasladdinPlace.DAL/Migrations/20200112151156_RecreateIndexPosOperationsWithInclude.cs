using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RecreateIndexPosOperationsWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropIndex( "IX_PosOperation_PosId_Include", "PosOperations" );

			migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX 
					[IX_PosOperation_PosId_Include] 
				ON [dbo].[PosOperations] 
					([PosId]) 
				INCLUDE (
					[AuditCompletionDateTime]
					, [AuditRequestDateTime]
					, [BonusAmount]
					, [Brand]
					, [CompletionInitiationDate]
					, [CorrectnessStatus]
					, [DateCompleted]
					, [DatePaid]
					, [DateStarted]
					, [Mode]
					, [Status]
					, [UserId]
					) WITH (
					PAD_INDEX = ON,
					FILLFACTOR = 90,
					ONLINE = OFF)" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropIndex( "IX_PosOperation_PosId_Include", "PosOperations" );

	        migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX [IX_PosOperation_PosId_Include] 
                ON [dbo].[PosOperations] ([PosId]) 
                INCLUDE ([UserId], [AuditCompletionDateTime], [AuditRequestDateTime],
                    [BonusAmount], [Brand], [DateCompleted],    
                    [DatePaid], [DateSentForVerification], [DateStarted], [Mode], [Status])" );
        }
    }
}
