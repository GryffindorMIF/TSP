using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ShoppingCartProductHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ShoppingCartProductHistory",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductDescription = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductPrice = table.Column<decimal>(nullable: false),
                    ProductPrimaryImageUrl = table.Column<string>(nullable: true),
                    ProductQuantity = table.Column<int>(nullable: false),
                    ShoppingCartId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartProductHistory", x => x.Id);
                    table.ForeignKey(
                        "FK_ShoppingCartProductHistory_ShoppingCart_ShoppingCartId",
                        x => x.ShoppingCartId,
                        "ShoppingCart",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_ShoppingCartProductHistory_ShoppingCartId",
                "ShoppingCartProductHistory",
                "ShoppingCartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ShoppingCartProductHistory");
        }
    }
}