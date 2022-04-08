using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddIndexForPosTemperaturesOnDateCreatedWithInclude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql( @" 
                CREATE NONCLUSTERED INDEX
					[IX_PosTemperatures_DateCreated_Include] 
				ON [dbo].[PosTemperatures]
					([DateCreated])
				INCLUDE (
					[PosId], 
					[Temperature]
					) WITH (
					PAD_INDEX = ON,
					FILLFACTOR = 90,
					ONLINE = OFF )" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropIndex( "IX_PosTemperatures_DateCreated_Include", "PosTemperatures" );
        }
    }
}
