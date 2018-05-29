using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class CategoryEntitiesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Category",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Category", x => x.Id); });

            migrationBuilder.CreateTable(
                "CategoryCategory",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    ParentCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCategory", x => x.Id);
                    table.ForeignKey(
                        "FK_CategoryCategory_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_CategoryCategory_Category_ParentCategoryId",
                        x => x.ParentCategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "ProductCategory",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.Id);
                    table.ForeignKey(
                        "FK_ProductCategory_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ProductCategory_Product_ProductId",
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
                "IX_ProductCategory_CategoryId",
                "ProductCategory",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_ProductCategory_ProductId",
                "ProductCategory",
                "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "CategoryCategory");

            migrationBuilder.DropTable(
                "ProductCategory");

            migrationBuilder.DropTable(
                "Category");
        }
    }
}