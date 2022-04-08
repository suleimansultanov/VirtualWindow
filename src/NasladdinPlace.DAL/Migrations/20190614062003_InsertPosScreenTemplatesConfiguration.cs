using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPosScreenTemplatesConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                  BEGIN TRANSACTION
                
                  INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                  VALUES (10000, 0, N'PosScreenTemplates', NULL, NULL),
                         (10001, 0, N'FilesCommonDirectoryName', NULL, 10000),
						 (10002, 0, N'TemplateDirectoryNameFormat', NULL, 10000),
						 (10003, 0, N'RequiredFilesList', NULL, 10000),
						 (10004, 3, N'DefaultPosScreenTemplateId', NULL, 10000)
                
                  INSERT INTO ConfigurationValues (KeyId, Value)
                  VALUES (10001, N'PosScreenTemplates'),
						 (10002, N'PosScreenTemplate_{0}'),
						 (10003, N'index.html'),
						 (10004, N'0')

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
