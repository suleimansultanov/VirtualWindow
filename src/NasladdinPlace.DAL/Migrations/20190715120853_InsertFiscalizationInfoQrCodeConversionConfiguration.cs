using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertFiscalizationInfoQrCodeConversionConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60100, 0, N'Fiscalization qr code', NULL, NULL)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60101, 3, N'Fiscalization qr code dimension size', NULL, 60100)

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (60101, 256)

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK  TRANSACTION
                END CATCH
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    DELETE FROM ConfigurationValues
                    WHERE KeyId = 60101

                    DELETE FROM ConfigurationKeys
                    WHERE Id IN (60100, 60101)

                    COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK  TRANSACTION
                END CATCH
            ");
        }
    }
}
