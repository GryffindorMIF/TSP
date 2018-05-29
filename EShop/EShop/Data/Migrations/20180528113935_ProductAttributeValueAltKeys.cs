using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class ProductAttributeValueAltKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_ProductAttributeValue_ProductId",
                "ProductAttributeValue");

            migrationBuilder.AddUniqueConstraint(
                "AlternateKey_ProductId_AttributeValueId",
                "ProductAttributeValue",
                new[] {"ProductId", "AttributeValueId"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                "AlternateKey_ProductId_AttributeValueId",
                "ProductAttributeValue");

            migrationBuilder.CreateIndex(
                "IX_ProductAttributeValue_ProductId",
                "ProductAttributeValue",
                "ProductId");
        }
    }
}