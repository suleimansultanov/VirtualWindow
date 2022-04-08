using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class SetCorrectFiscalizationTypeToFiscalizationInfosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                DECLARE @fiscalizationPending INT = 0;
                DECLARE @fiscalizationSuccess INT = 1;
                DECLARE @fiscalizationPendingError INT = 2;
                
                DECLARE @fiscalizationCorrectionPending INT = 3;
                DECLARE @fiscalizationCorrectionSuccess  INT = 4;
                DECLARE @fiscalizationCorrectionError  INT = 5;             
                
                DECLARE @fiscalizationTypeCorrection INT = 2;

                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                
                BEGIN TRY
                  BEGIN TRANSACTION
             
                    UPDATE CorrectionFiscalizationInfos
                    SET State = (
                        CASE
                          WHEN State = @fiscalizationCorrectionPending THEN @fiscalizationPending
                          WHEN State = @fiscalizationCorrectionSuccess THEN @fiscalizationSuccess
                          WHEN State = @fiscalizationCorrectionError THEN @fiscalizationPendingError
                        END),
                        Type =  @fiscalizationTypeCorrection
						FROM (SELECT * FROM FiscalizationInfos WHERE (State = @fiscalizationCorrectionPending 
											OR State = @fiscalizationCorrectionSuccess 
											OR State = @fiscalizationCorrectionError)) AS CorrectionFiscalizationInfos
                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // intentionally left empty
        }
    }
}
