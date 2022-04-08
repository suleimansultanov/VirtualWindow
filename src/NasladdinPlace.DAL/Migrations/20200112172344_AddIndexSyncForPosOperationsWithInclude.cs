using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexSyncForPosOperationsWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX 
					[IX_PosOperation_Mode_DateCompleted_Include]
				ON [dbo].[PosOperations] 
					([Mode],[DateCompleted])
				INCLUDE (
					[UserId]
					,[CompletionInitiationDate]
					,[DateStarted]
					,[DatePaid]
					,[BonusAmount]
					,[Status]
					,[Brand]
					,[AuditRequestDateTime]
					,[AuditCompletionDateTime]
					,[CorrectnessStatus]
					) WITH (
					PAD_INDEX = ON,
					FILLFACTOR = 90,
					ONLINE = OFF)" );
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropIndex( "IX_PosOperation_Mode_DateCompleted_Include", "PosOperations" );
        }
    }
}
