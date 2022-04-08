using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class AddConfigurationKeyValueTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ValueDataType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationKeys_ConfigurationKeys_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ConfigurationKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KeyId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationValues_ConfigurationKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "ConfigurationKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationKeys_ParentId",
                table: "ConfigurationKeys",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationValues_KeyId",
                table: "ConfigurationValues",
                column: "KeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationValues");

            migrationBuilder.DropTable(
                name: "ConfigurationKeys");
        }
    }
}
