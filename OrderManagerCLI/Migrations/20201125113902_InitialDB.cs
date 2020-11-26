using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderManagerCLI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Price", "ProductName", "Quantity" },
                values: new object[,]
                {
                    { "b4b5a9c9-70c5-4e91-b0c6-e38c94475659", 10m, "Ürün 1", 100 },
                    { "f5ed9460-f063-4b0d-92fb-d0e605c65457", 10m, "Ürün 2", 100 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
