using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexForPosTemperaturesWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX
					[IX_PosTemperatures_PosId_DateCreated_Include] 
				ON [dbo].[PosTemperatures]
					([PosId], [DateCreated]) 
				INCLUDE ([Temperature])" );
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropIndex( "IX_PosTemperatures_PosId_DateCreated_Include", "PosTemperatures" );
        }
    }
}
