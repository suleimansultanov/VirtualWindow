using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddApproachApiConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (10300, 0, N'ApproachesApiSettings', NULL, NULL),
                         (10301, 0, N'ApproachesDataDocumentUri', NULL, 10300),
                         (10302, 0, N'ApproachesDataDocumentSheetName', NULL, 10300),
                         (10303, 3, N'ApproachesDataCacheLifeTimeInSeconds', NULL, 10300)

                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (10301, N'https://docs.google.com/spreadsheets/d/1XKp2Vl_uwBCHvRhihD7zl9YA-f28m7mNSpnnEKUapWc/edit#gid=1184580428'),
                         (10302, N'Approaches'),
                         (10303, N'60')

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
