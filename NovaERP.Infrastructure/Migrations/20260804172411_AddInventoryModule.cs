using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuantityOnHand = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    QuantityReserved = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    QuantityAvailable = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    ReorderLevel = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    MinimumLevel = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    MaximumLevel = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    LastStockUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventories_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventories_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    ReferenceType = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_WarehouseId",
                table: "Inventories",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_WarehouseLocationId",
                table: "Inventories",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Product_Warehouse_Location",
                table: "Inventories",
                columns: new[] { "ProductId", "WarehouseId", "WarehouseLocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_InventoryId",
                table: "InventoryTransactions",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_ReferenceId",
                table: "InventoryTransactions",
                column: "ReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "Inventories");
        }
    }
}
