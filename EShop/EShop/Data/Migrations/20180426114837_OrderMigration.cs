using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class OrderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                "TotalPrice",
                "Order",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                "AddressId",
                "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                "CardNumber",
                "Order",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                "ConfirmationDate",
                "Order",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                "PurchaseDate",
                "Order",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                "ShoppingCartId",
                "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                "CardInfo",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    CardNumber = table.Column<string>(nullable: false),
                    ExpMonth = table.Column<int>(nullable: false),
                    ExpYear = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardInfo", x => x.Id);
                    table.ForeignKey(
                        "FK_CardInfo_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "DeliveryAddress",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    County = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Zipcode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddress", x => x.Id);
                    table.ForeignKey(
                        "FK_DeliveryAddress_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Order_AddressId",
                "Order",
                "AddressId");

            migrationBuilder.CreateIndex(
                "IX_Order_ShoppingCartId",
                "Order",
                "ShoppingCartId");

            migrationBuilder.CreateIndex(
                "IX_CardInfo_UserId",
                "CardInfo",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_DeliveryAddress_UserId",
                "DeliveryAddress",
                "UserId");

            migrationBuilder.AddForeignKey(
                "FK_Order_DeliveryAddress_AddressId",
                "Order",
                "AddressId",
                "DeliveryAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Order_ShoppingCart_ShoppingCartId",
                "Order",
                "ShoppingCartId",
                "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Order_DeliveryAddress_AddressId",
                "Order");

            migrationBuilder.DropForeignKey(
                "FK_Order_ShoppingCart_ShoppingCartId",
                "Order");

            migrationBuilder.DropTable(
                "CardInfo");

            migrationBuilder.DropTable(
                "DeliveryAddress");

            migrationBuilder.DropIndex(
                "IX_Order_AddressId",
                "Order");

            migrationBuilder.DropIndex(
                "IX_Order_ShoppingCartId",
                "Order");

            migrationBuilder.DropColumn(
                "AddressId",
                "Order");

            migrationBuilder.DropColumn(
                "CardNumber",
                "Order");

            migrationBuilder.DropColumn(
                "ConfirmationDate",
                "Order");

            migrationBuilder.DropColumn(
                "PurchaseDate",
                "Order");

            migrationBuilder.DropColumn(
                "ShoppingCartId",
                "Order");

            migrationBuilder.AlterColumn<decimal>(
                "TotalPrice",
                "Order",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}