using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class OrderAddressBugfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Order_DeliveryAddress_AddressId",
                "Order");

            migrationBuilder.DropIndex(
                "IX_Order_AddressId",
                "Order");

            migrationBuilder.DropColumn(
                "AddressId",
                "Order");

            migrationBuilder.AddColumn<string>(
                "Address",
                "Order",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Address",
                "Order");

            migrationBuilder.AddColumn<int>(
                "AddressId",
                "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                "IX_Order_AddressId",
                "Order",
                "AddressId");

            migrationBuilder.AddForeignKey(
                "FK_Order_DeliveryAddress_AddressId",
                "Order",
                "AddressId",
                "DeliveryAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}