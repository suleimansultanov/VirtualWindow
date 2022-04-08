using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddFiscalizationInfosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FiscalizationInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestId = table.Column<Guid>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    DateTimeRequest = table.Column<DateTime>(nullable: false),
                    DateTimeResponse = table.Column<DateTime>(nullable: true),
                    ErrorInfo = table.Column<string>(nullable: true),
                    DocumentInfo = table.Column<string>(nullable: true),
                    QrCodeData = table.Column<string>(nullable: true),
                    QrCodeFormat = table.Column<string>(nullable: true),
                    FiscalizationNumber = table.Column<string>(nullable: true),
                    FiscalizationSerial = table.Column<string>(nullable: true),
                    FiscalizationSign = table.Column<string>(nullable: true),
                    DocumentDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalizationInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalizationInfos_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiscalizationInfos_PosOperationId_PosId",
                table: "FiscalizationInfos",
                columns: new[] { "PosOperationId", "PosId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FiscalizationInfos");
        }
    }
}
