using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EShop.Data.Migrations
{
    public partial class cardinfoupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardInfo_Users_UserId",
                table: "CardInfo");

            migrationBuilder.DropIndex(
                name: "IX_CardInfo_UserId",
                table: "CardInfo");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ExpYear",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "ExpMonth",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "ExpYear",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "ExpMonth",
                table: "CardInfo",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_CardInfo_UserId",
                table: "CardInfo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardInfo_Users_UserId",
                table: "CardInfo",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
