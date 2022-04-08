using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NasladdinPlace.DAL.Migrations
{
    public partial class RenameInventoryTableToDocumentGoodsMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryDocumentLabeledGoods");

            migrationBuilder.DropTable(
                name: "InventoryDocumentTableItems");

            migrationBuilder.DropTable(
                name: "InventoryDocuments");

            migrationBuilder.CreateTable(
                name: "DocumentsGoodsMoving",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentNumber = table.Column<string>(nullable: true),
                    IsPosted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: false),
                    PosId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsGoodsMoving", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsGoodsMoving_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentGoodsMovingTableItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentId = table.Column<int>(nullable: false),
                    LineNum = table.Column<int>(nullable: false),
                    GoodId = table.Column<int>(nullable: true),
                    BalanceAtBegining = table.Column<int>(nullable: false),
                    BalanceAtEnd = table.Column<int>(nullable: false),
                    Income = table.Column<int>(nullable: false),
                    Outcome = table.Column<int>(nullable: false),
                    LabelsAtBegining = table.Column<string>(nullable: true),
                    LabelsAtEnd = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentGoodsMovingTableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentGoodsMovingTableItems_DocumentsGoodsMoving_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "DocumentsGoodsMoving",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentGoodsMovingTableItems_Goods_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentGoodsMovingLabeledGoods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentTableItemId = table.Column<int>(nullable: false),
                    LabeledGoodId = table.Column<int>(nullable: false),
                    BalanceType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentGoodsMovingLabeledGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentGoodsMovingLabeledGoods_DocumentGoodsMovingTableItems_DocumentTableItemId",
                        column: x => x.DocumentTableItemId,
                        principalTable: "DocumentGoodsMovingTableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentGoodsMovingLabeledGoods_LabeledGoods_LabeledGoodId",
                        column: x => x.LabeledGoodId,
                        principalTable: "LabeledGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentGoodsMovingLabeledGoods_DocumentTableItemId",
                table: "DocumentGoodsMovingLabeledGoods",
                column: "DocumentTableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentGoodsMovingLabeledGoods_LabeledGoodId",
                table: "DocumentGoodsMovingLabeledGoods",
                column: "LabeledGoodId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentGoodsMovingTableItems_DocumentId",
                table: "DocumentGoodsMovingTableItems",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentGoodsMovingTableItems_GoodId",
                table: "DocumentGoodsMovingTableItems",
                column: "GoodId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsGoodsMoving_PosOperationId_PosId",
                table: "DocumentsGoodsMoving",
                columns: new[] { "PosOperationId", "PosId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentGoodsMovingLabeledGoods");

            migrationBuilder.DropTable(
                name: "DocumentGoodsMovingTableItems");

            migrationBuilder.DropTable(
                name: "DocumentsGoodsMoving");

            migrationBuilder.CreateTable(
                name: "InventoryDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentNumber = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsPosted = table.Column<bool>(nullable: false),
                    PosId = table.Column<int>(nullable: false),
                    PosOperationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryDocuments_PosOperations_PosOperationId_PosId",
                        columns: x => new { x.PosOperationId, x.PosId },
                        principalTable: "PosOperations",
                        principalColumns: new[] { "Id", "PosId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDocumentTableItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BalanceAtBegining = table.Column<int>(nullable: false),
                    BalanceAtEnd = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    GoodId = table.Column<int>(nullable: false),
                    Income = table.Column<int>(nullable: false),
                    LabelsAtBegining = table.Column<string>(nullable: true),
                    LabelsAtEnd = table.Column<string>(nullable: true),
                    LineNum = table.Column<int>(nullable: false),
                    Outcome = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDocumentTableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryDocumentTableItems_InventoryDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "InventoryDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryDocumentTableItems_Goods_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDocumentLabeledGoods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BalanceType = table.Column<int>(nullable: false),
                    DocumentTableItemId = table.Column<int>(nullable: false),
                    LabeledGoodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDocumentLabeledGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryDocumentLabeledGoods_InventoryDocumentTableItems_DocumentTableItemId",
                        column: x => x.DocumentTableItemId,
                        principalTable: "InventoryDocumentTableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryDocumentLabeledGoods_LabeledGoods_LabeledGoodId",
                        column: x => x.LabeledGoodId,
                        principalTable: "LabeledGoods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLabeledGoods_DocumentTableItemId",
                table: "InventoryDocumentLabeledGoods",
                column: "DocumentTableItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLabeledGoods_LabeledGoodId",
                table: "InventoryDocumentLabeledGoods",
                column: "LabeledGoodId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocuments_PosOperationId_PosId",
                table: "InventoryDocuments",
                columns: new[] { "PosOperationId", "PosId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentTableItems_DocumentId",
                table: "InventoryDocumentTableItems",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentTableItems_GoodId",
                table: "InventoryDocumentTableItems",
                column: "GoodId");
        }
    }
}
