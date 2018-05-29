using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class AttributeConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_ProductAttributeValue_ProductId",
                "ProductAttributeValue");

            migrationBuilder.DropIndex(
                "IX_CategoryAttribute_CategoryId",
                "CategoryAttribute");

            migrationBuilder.AddUniqueConstraint(
                "AlternateKey_ProductId_AttributeValueId",
                "ProductAttributeValue",
                new[] {"ProductId", "AttributeValueId"});

            migrationBuilder.AddUniqueConstraint(
                "AlternateKey_CategoryId_AttributeValueId",
                "CategoryAttribute",
                new[] {"CategoryId", "AttributeValueId"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                "AlternateKey_ProductId_AttributeValueId",
                "ProductAttributeValue");

            migrationBuilder.DropUniqueConstraint(
                "AlternateKey_CategoryId_AttributeValueId",
                "CategoryAttribute");

            migrationBuilder.CreateIndex(
                "IX_ProductAttributeValue_ProductId",
                "ProductAttributeValue",
                "ProductId");

            migrationBuilder.CreateIndex(
                "IX_CategoryAttribute_CategoryId",
                "CategoryAttribute",
                "CategoryId");
        }
    }
}