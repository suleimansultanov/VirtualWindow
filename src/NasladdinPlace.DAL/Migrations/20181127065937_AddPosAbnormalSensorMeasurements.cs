using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddPosAbnormalSensorMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosAbnormalSensorMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PosId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    MeasurementValue = table.Column<double>(nullable: false, defaultValue: 0.0),
                    MeasurementUnit = table.Column<int>(nullable: false),
                    DateMeasured = table.Column<DateTime>(nullable: false),
                    SensorPosition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosAbnormalSensorMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosAbnormalSensorMeasurements_PointsOfSale_PosId",
                        column: x => x.PosId,
                        principalTable: "PointsOfSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosAbnormalSensorMeasurements_PosId",
                table: "PosAbnormalSensorMeasurements",
                column: "PosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosAbnormalSensorMeasurements");
        }
    }
}
