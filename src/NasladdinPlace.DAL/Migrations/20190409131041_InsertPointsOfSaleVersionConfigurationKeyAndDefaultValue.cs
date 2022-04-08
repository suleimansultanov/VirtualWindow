using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPointsOfSaleVersionConfigurationKeyAndDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                VALUES (0, 0, N'PointsOfSale', NULL, NULL)
                
                INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                VALUES (1, 0, N'PointsOfSaleRequiredMinVersion', NULL, 0)
                
                INSERT INTO ConfigurationValues (KeyId, Value)
                VALUES (1, N'1.0.0.0')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Do nothing because this is a migration only sql script.
        }
    }
}
