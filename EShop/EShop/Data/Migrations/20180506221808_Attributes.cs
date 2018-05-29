using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class Attributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Attribute",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Attribute", x => x.Id); });

            migrationBuilder.CreateTable(
                "AttributeValue",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeCategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValue", x => x.Id);
                    table.ForeignKey(
                        "FK_AttributeValue_Attribute_AttributeCategoryId",
                        x => x.AttributeCategoryId,
                        "Attribute",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "CategoryAttribute",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeCategoryId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttribute", x => x.Id);
                    table.ForeignKey(
                        "FK_CategoryAttribute_AttributeValue_AttributeCategoryId",
                        x => x.AttributeCategoryId,
                        "AttributeValue",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_CategoryAttribute_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ProductAttributeValue",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        "FK_ProductAttributeValue_AttributeValue_AttributeId",
                        x => x.AttributeId,
                        "AttributeValue",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ProductAttributeValue_Product_ProductId",
                        x => x.ProductId,
                        "Product",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_AttributeValue_AttributeCategoryId",
                "AttributeValue",
                "AttributeCategoryId");

            migrationBuilder.CreateIndex(
                "IX_CategoryAttribute_AttributeCategoryId",
                "CategoryAttribute",
                "AttributeCategoryId");

            migrationBuilder.CreateIndex(
                "IX_CategoryAttribute_CategoryId",
                "CategoryAttribute",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_ProductAttributeValue_AttributeId",
                "ProductAttributeValue",
                "AttributeId");

            migrationBuilder.CreateIndex(
                "IX_ProductAttributeValue_ProductId",
                "ProductAttributeValue",
                "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "CategoryAttribute");

            migrationBuilder.DropTable(
                "ProductAttributeValue");

            migrationBuilder.DropTable(
                "AttributeValue");

            migrationBuilder.DropTable(
                "Attribute");
        }
    }
}