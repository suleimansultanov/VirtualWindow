using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class SetValuesToTotalFiscalizationAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

                    BEGIN TRANSACTION

                        BEGIN TRY
                                       
                            UPDATE FiscalizationInfos
                            SET TotalFiscalizationAmount = CAST(LTRIM(RTRIM(SUBSTRING (DocumentInfo, 
											                        CHARINDEX(N'##BIG##ИТОГ', DocumentInfo) + 13, 
											                        CHARINDEX(CHAR(10), DocumentInfo, CHARINDEX(N'##BIG##ИТОГ', DocumentInfo)) - CHARINDEX(N'##BIG##ИТОГ',DocumentInfo) - 13)))
									                        AS DECIMAL(18,2))
	                        WHERE CorrectionAmount IS NULL AND DocumentInfo IS NOT NULL
                    
                            COMMIT TRANSACTION
                    
                        END TRY
                        BEGIN CATCH
                        ROLLBACK TRANSACTION
                        END CATCH"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
