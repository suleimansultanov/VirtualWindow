using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexPosOperationsDateCompleteDesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@" 
                CREATE NONCLUSTERED INDEX [IX_PosOperations_DateCompleteDesc_Id_PosId] ON [dbo].[PosOperations]
                (
	                [DateCompleted] DESC,
	                [Id] ASC,
	                [PosId] ASC
                )");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropIndex("IX_PosOperations_DateCompleteDesc_Id_PosId", "PosOperations");
		}
    }
}
