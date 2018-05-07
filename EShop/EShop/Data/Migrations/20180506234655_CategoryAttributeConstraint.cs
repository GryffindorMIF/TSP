using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class CategoryAttributeConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategoryAttribute_CategoryId",
                table: "CategoryAttribute");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_CategoryId_AttributeValueId",
                table: "CategoryAttribute",
                columns: new[] { "CategoryId", "AttributeValueId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_CategoryId_AttributeValueId",
                table: "CategoryAttribute");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttribute_CategoryId",
                table: "CategoryAttribute",
                column: "CategoryId");
        }
    }
}
