using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class AttributesRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_AttributeValue_Attribute_AttributeCategoryId",
                "AttributeValue");

            migrationBuilder.DropForeignKey(
                "FK_CategoryAttribute_AttributeValue_AttributeCategoryId",
                "CategoryAttribute");

            migrationBuilder.DropForeignKey(
                "FK_ProductAttributeValue_AttributeValue_AttributeId",
                "ProductAttributeValue");

            migrationBuilder.DropTable(
                "Attribute");

            migrationBuilder.DropPrimaryKey(
                "PK_AttributeValue",
                "AttributeValue");

            migrationBuilder.DropIndex(
                "IX_AttributeValue_AttributeCategoryId",
                "AttributeValue");

            migrationBuilder.DropColumn(
                "AttributeCategoryId",
                "AttributeValue");

            migrationBuilder.RenameTable(
                "AttributeValue",
                newName: "Attribute");

            migrationBuilder.RenameColumn(
                "AttributeId",
                "ProductAttributeValue",
                "AttributeValueId");

            migrationBuilder.RenameIndex(
                "IX_ProductAttributeValue_AttributeId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_AttributeValueId");

            migrationBuilder.RenameColumn(
                "AttributeCategoryId",
                "CategoryAttribute",
                "AttributeValueId");

            migrationBuilder.RenameIndex(
                "IX_CategoryAttribute_AttributeCategoryId",
                table: "CategoryAttribute",
                newName: "IX_CategoryAttribute_AttributeValueId");

            migrationBuilder.AddPrimaryKey(
                "PK_Attribute",
                "Attribute",
                "Id");

            migrationBuilder.CreateTable(
                "AttributeValue",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValue", x => x.Id);
                    table.ForeignKey(
                        "FK_AttributeValue_Attribute_AttributeId",
                        x => x.AttributeId,
                        "Attribute",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_AttributeValue_AttributeId",
                "AttributeValue",
                "AttributeId");

            migrationBuilder.AddForeignKey(
                "FK_CategoryAttribute_AttributeValue_AttributeValueId",
                "CategoryAttribute",
                "AttributeValueId",
                "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_ProductAttributeValue_AttributeValue_AttributeValueId",
                "ProductAttributeValue",
                "AttributeValueId",
                "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_CategoryAttribute_AttributeValue_AttributeValueId",
                "CategoryAttribute");

            migrationBuilder.DropForeignKey(
                "FK_ProductAttributeValue_AttributeValue_AttributeValueId",
                "ProductAttributeValue");

            migrationBuilder.DropTable(
                "AttributeValue");

            migrationBuilder.DropPrimaryKey(
                "PK_Attribute",
                "Attribute");

            migrationBuilder.RenameTable(
                "Attribute",
                newName: "AttributeValue");

            migrationBuilder.RenameColumn(
                "AttributeValueId",
                "ProductAttributeValue",
                "AttributeId");

            migrationBuilder.RenameIndex(
                "IX_ProductAttributeValue_AttributeValueId",
                table: "ProductAttributeValue",
                newName: "IX_ProductAttributeValue_AttributeId");

            migrationBuilder.RenameColumn(
                "AttributeValueId",
                "CategoryAttribute",
                "AttributeCategoryId");

            migrationBuilder.RenameIndex(
                "IX_CategoryAttribute_AttributeValueId",
                table: "CategoryAttribute",
                newName: "IX_CategoryAttribute_AttributeCategoryId");

            migrationBuilder.AddColumn<int>(
                "AttributeCategoryId",
                "AttributeValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                "PK_AttributeValue",
                "AttributeValue",
                "Id");

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

            migrationBuilder.CreateIndex(
                "IX_AttributeValue_AttributeCategoryId",
                "AttributeValue",
                "AttributeCategoryId");

            migrationBuilder.AddForeignKey(
                "FK_AttributeValue_Attribute_AttributeCategoryId",
                "AttributeValue",
                "AttributeCategoryId",
                "Attribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_CategoryAttribute_AttributeValue_AttributeCategoryId",
                "CategoryAttribute",
                "AttributeCategoryId",
                "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_ProductAttributeValue_AttributeValue_AttributeId",
                "ProductAttributeValue",
                "AttributeId",
                "AttributeValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}