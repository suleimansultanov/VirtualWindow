using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class InsertPosSensorMeasurementsAbnormalValuesConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
                BEGIN TRY
                    BEGIN TRANSACTION

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (10200, 0, N'PosSensorMeasurementsSettings', NULL, NULL)

                    INSERT INTO ConfigurationKeys (Id, ValueDataType, Name, Description, ParentId)
                    VALUES (10201, 5, N'LowerNormalAmperage', NULL, 10200),
                           (10202, 5, N'UpperNormalAmperage', NULL, 10200),
                           (10203, 3, N'FrontPanelPositionAbnormalValue', NULL, 10200)
                            

                    INSERT INTO ConfigurationValues (KeyId, Value)
                    VALUES (10201, N'1.00'),
                           (10202, N'2.00'),
                           (10203, N'1')

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
