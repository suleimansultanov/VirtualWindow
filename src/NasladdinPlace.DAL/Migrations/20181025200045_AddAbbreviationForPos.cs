using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddAbbreviationForPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbbreviatedName",
                table: "PointsOfSale",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                  @"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                    BEGIN TRY
                        BEGIN TRANSACTION 
	                        UPDATE [PointsOfSale] SET [AbbreviatedName] = SUBSTRING([Name], 1, 3)
                        COMMIT
                    END TRY
                    BEGIN CATCH
                        ROLLBACK
                    END CATCH");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbbreviatedName",
                table: "PointsOfSale");
        }
    }
}
