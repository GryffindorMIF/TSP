using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class AttributesRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeValue_Attribute_AttributeCategoryId",
                table: "AttributeValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAttribute_AttributeValue_AttributeCategoryId",
                table: "CategoryAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_AttributeValue_AttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "Attribute");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeValue",
                table: "AttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_AttributeValue_AttributeCategoryId",
                table: "AttributeValue");

            migrationBuilder.DropColumn(
                name: "AttributeCategoryId",
                table: "AttributeValue");

            migrationBuilder.RenameTable(
                name: "AttributeValue",
                newName: "Attribute");

            migrationBuilder.RenameColumn(
                name: "AttributeId",
                table: "ProductAttributeValue",
                newName: "AttributeValueId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValue_AttributeId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_AttributeValueId");

            migrationBuilder.RenameColumn(
                name: "AttributeCategoryId",
                table: "CategoryAttribute",
                newName: "AttributeValueId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryAttribute_AttributeCategoryId",
                table: "CategoryAttribute",
                newName: "IX_CategoryAttribute_AttributeValueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attribute",
                table: "Attribute",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeValue_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_AttributeId",
                table: "AttributeValue",
                column: "AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAttribute_AttributeValue_AttributeValueId",
                table: "CategoryAttribute",
                column: "AttributeValueId",
                principalTable: "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_AttributeValue_AttributeValueId",
                table: "ProductAttributeValue",
                column: "AttributeValueId",
                principalTable: "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryAttribute_AttributeValue_AttributeValueId",
                table: "CategoryAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_AttributeValue_AttributeValueId",
                table: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "AttributeValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attribute",
                table: "Attribute");

            migrationBuilder.RenameTable(
                name: "Attribute",
                newName: "AttributeValue");

            migrationBuilder.RenameColumn(
                name: "AttributeValueId",
                table: "ProductAttributeValue",
                newName: "AttributeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttributeValue_AttributeValueId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_AttributeId");

            migrationBuilder.RenameColumn(
                name: "AttributeValueId",
                table: "CategoryAttribute",
                newName: "AttributeCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryAttribute_AttributeValueId",
                table: "CategoryAttribute",
                newName: "IX_CategoryAttribute_AttributeCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "AttributeCategoryId",
                table: "AttributeValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeValue",
                table: "AttributeValue",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribute", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValue_AttributeCategoryId",
                table: "AttributeValue",
                column: "AttributeCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeValue_Attribute_AttributeCategoryId",
                table: "AttributeValue",
                column: "AttributeCategoryId",
                principalTable: "Attribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryAttribute_AttributeValue_AttributeCategoryId",
                table: "CategoryAttribute",
                column: "AttributeCategoryId",
                principalTable: "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_AttributeValue_AttributeId",
                table: "ProductAttributeValue",
                column: "AttributeId",
                principalTable: "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
