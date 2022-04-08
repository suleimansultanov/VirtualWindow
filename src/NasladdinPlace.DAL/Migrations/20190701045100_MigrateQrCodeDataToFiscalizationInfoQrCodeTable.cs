using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class MigrateQrCodeDataToFiscalizationInfoQrCodeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

                BEGIN TRY
                    BEGIN TRANSACTION
		                SET NOCOUNT ON

		                INSERT INTO FiscalizationInfoQrCode WITH(TABLOCK)
		                (
		                   Id, 
		                   QrCodeData, 
		                   QrCodeFormat
		                )
		                SELECT Id, QrCodeData, QrCodeFormat
                        FROM FiscalizationInfos
                        WHERE QrCodeData IS NOT NULL AND QrCodeFormat IS NOT NULL

		                SET NOCOUNT OFF

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
