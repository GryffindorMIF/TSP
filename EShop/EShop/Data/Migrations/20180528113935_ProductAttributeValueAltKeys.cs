using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class ProductAttributeValueAltKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_ProductId_AttributeValueId",
                table: "ProductAttributeValue",
                columns: new[] { "ProductId", "AttributeValueId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_ProductId_AttributeValueId",
                table: "ProductAttributeValue");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId");
        }
    }
}
