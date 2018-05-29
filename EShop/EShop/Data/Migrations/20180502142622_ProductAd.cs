using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ProductAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ProductAd",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    AdImageUrl = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAd", x => x.Id);
                    table.ForeignKey(
                        "FK_ProductAd_Product_ProductId",
                        x => x.ProductId,
                        "Product",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_CategoryCategory_CategoryId",
                "CategoryCategory",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_CategoryCategory_ParentCategoryId",
                "CategoryCategory",
                "ParentCategoryId");

            migrationBuilder.CreateIndex(
                "IX_ProductAd_ProductId",
                "ProductAd",
                "ProductId");

            migrationBuilder.AddForeignKey(
                "FK_CategoryCategory_Category_CategoryId",
                "CategoryCategory",
                "CategoryId",
                "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_CategoryCategory_Category_ParentCategoryId",
                "CategoryCategory",
                "ParentCategoryId",
                "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_CategoryCategory_Category_CategoryId",
                "CategoryCategory");

            migrationBuilder.DropForeignKey(
                "FK_CategoryCategory_Category_ParentCategoryId",
                "CategoryCategory");

            migrationBuilder.DropTable(
                "ProductAd");

            migrationBuilder.DropIndex(
                "IX_CategoryCategory_CategoryId",
                "CategoryCategory");

            migrationBuilder.DropIndex(
                "IX_CategoryCategory_ParentCategoryId",
                "CategoryCategory");
        }
    }
}