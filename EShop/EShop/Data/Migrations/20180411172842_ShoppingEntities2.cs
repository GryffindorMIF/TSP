using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ShoppingEntities2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Order",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    StatusCode = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Order", x => x.Id); });

            migrationBuilder.CreateTable(
                "ShoppingCart",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table => { table.PrimaryKey("PK_ShoppingCart", x => x.Id); });

            migrationBuilder.CreateTable(
                "ShoppingCartProduct",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    ShoppingCartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartProduct", x => x.Id);
                    table.ForeignKey(
                        "FK_ShoppingCartProduct_Product_ProductId",
                        x => x.ProductId,
                        "Product",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_ShoppingCartProduct_ShoppingCart_ShoppingCartId",
                        x => x.ShoppingCartId,
                        "ShoppingCart",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_ShoppingCartProduct_ProductId",
                "ShoppingCartProduct",
                "ProductId");

            migrationBuilder.CreateIndex(
                "IX_ShoppingCartProduct_ShoppingCartId",
                "ShoppingCartProduct",
                "ShoppingCartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Order");

            migrationBuilder.DropTable(
                "ShoppingCartProduct");

            migrationBuilder.DropTable(
                "ShoppingCart");
        }
    }
}