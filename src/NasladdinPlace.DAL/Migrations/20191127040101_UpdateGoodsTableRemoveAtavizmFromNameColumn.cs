using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class UpdateGoodsTableRemoveAtavizmFromNameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  UPDATE [Goods]
                  SET Name = CASE
			                      WHEN Name LIKE '%' + CHAR(10) + '.'
			      		                THEN LTRIM(RTRIM(REPLACE(Name, CHAR(10) + '.', '')))
                  
			                      WHEN Name LIKE '%' + CHAR(10) + '-'
			      		                THEN LTRIM(RTRIM(REPLACE(Name, CHAR(10) + '-', '')))
                  
			                      WHEN Name LIKE '% ' + CHAR(10) + '%' OR Name LIKE '%' + CHAR(10) + ' %'
			      		                THEN LTRIM(RTRIM(REPLACE(Name, CHAR(10), '')))
			                      
			                      WHEN Name LIKE '%' + CHAR(160) + '%' AND Name LIKE '%' + CHAR(10) + '%'
			      		                THEN LTRIM(RTRIM(REPLACE(REPLACE(Name, CHAR(160), ''), CHAR(10), ' ')))
		                  
			                      WHEN Name LIKE '%' + CHAR(10) + '%'
			      		                THEN LTRIM(RTRIM(REPLACE(Name, CHAR(10), ' ')))
                  
			                      ELSE LTRIM(RTRIM(Name))
			                  END

                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
