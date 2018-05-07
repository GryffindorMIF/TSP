using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class AttributeConstraintsRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_ProductId_AttributeValueId",
                table: "ProductAttributeValue");

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_CategoryId_AttributeValueId",
                table: "CategoryAttribute");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_CategoryId",
                table: "CategoryAttribute",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAttribute_CategoryId",
                table: "CategoryAttribute");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_ProductId_AttributeValueId",
                table: "ProductAttributeValue",
                columns: new[] { "ProductId", "AttributeValueId" });

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_CategoryId_AttributeValueId",
                table: "CategoryAttribute",
                columns: new[] { "CategoryId", "AttributeValueId" });
        }
    }
}
