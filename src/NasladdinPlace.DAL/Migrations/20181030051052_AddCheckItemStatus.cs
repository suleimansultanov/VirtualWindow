using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddCheckItemStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CheckItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
              @"DECLARE @actionNoAction INT = 0;
                DECLARE @actionRefunded INT = 1;
                DECLARE @actionDeleted INT = 2;
                
                DECLARE @statusNotPaid INT = 1;
                DECLARE @statusPaid INT = 2;
                DECLARE @statusRefunded INT = 3;
                DECLARE @statusDeleted INT = 4;
                
                DECLARE @operationStatusPaid INT = 4;
                
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                
                BEGIN TRY
                  BEGIN TRANSACTION
                
                    UPDATE CheckItems
                    SET Status = (
                        CASE
                          WHEN Action = @actionRefunded THEN @statusRefunded
                          WHEN Action = @actionDeleted THEN @statusDeleted
                          WHEN Action = @actionNoAction AND (
                            (SELECT TOP 1 PO.Status
                             FROM PosOperations PO
                             WHERE PO.Id = PosOperationId) = @operationStatusPaid
                            )
                                  THEN @statusPaid
                          ELSE @statusNotPaid
                            END
                        )
                
                  COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                  ROLLBACK TRANSACTION
                END CATCH"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CheckItems");
        }
    }
}
