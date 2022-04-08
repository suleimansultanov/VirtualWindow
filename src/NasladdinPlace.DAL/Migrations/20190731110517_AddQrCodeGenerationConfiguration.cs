using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddQrCodeGenerationConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY 
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60200, 0, N'PosTokenServices', NULL, NULL)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (60201, 1, N'PosTokenServicesUpdatePeriod', NULL, 60200),
                        (60202, 0, N'PosTokenServicesEncryptionKey', NULL, 60200),
                        (60203, 0, N'PosTokenServicesPrefix', NULL, 60200),
                        (60204, 1, N'PosTokenServicesTokenValidityPeriod', NULL, 60200),
                        (60205, 1, N'PosTokenServicesTokenProviderCachePeriod', NULL, 60200)

                    INSERT INTO ConfigurationValues (KeyId, [Value]) 
                    VALUES (60201, N'00:01:00'),
                        (60202, N'Nasladdin2019'),
                        (60203, N'/Stores/'),
                        (60204, N'00:03:00'),
                        (60205, N'00:30:00')

                    COMMIT TRANSACTION
                END TRY 
                BEGIN CATCH 
                    ROLLBACK TRANSACTION
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
                    WHERE ConfigurationValues.KeyId IN (60201, 60202, 60203, 60204, 60205)

                    DELETE FROM ConfigurationKeys 
                    WHERE ConfigurationKeys.Id IN (60201, 60202, 60203, 60204, 60205)

                    DELETE FROM ConfigurationKeys
                    WHERE Id = 60200

                    COMMIT TRANSACTION
                END TRY 
                BEGIN CATCH 
                    ROLLBACK TRANSACTION
                END CATCH
            ");
        }
    }
}
