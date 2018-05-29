using Microsoft.EntityFrameworkCore.Migrations;

namespace EShop.Data.Migrations
{
    public partial class TableNamesEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                "FK_AspNetUserClaims_AspNetUsers_UserId",
                "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                "FK_AspNetUserLogins_AspNetUsers_UserId",
                "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                "FK_AspNetUserRoles_AspNetRoles_RoleId",
                "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                "FK_AspNetUserRoles_AspNetUsers_UserId",
                "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                "FK_AspNetUserTokens_AspNetUsers_UserId",
                "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetUserTokens",
                "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetUsers",
                "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetUserRoles",
                "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetUserLogins",
                "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetUserClaims",
                "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetRoles",
                "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                "PK_AspNetRoleClaims",
                "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                "AspNetUserTokens",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                "AspNetUsers",
                newName: "Users");

            migrationBuilder.RenameTable(
                "AspNetUserRoles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                "AspNetUserLogins",
                newName: "UserLogins");

            migrationBuilder.RenameTable(
                "AspNetUserClaims",
                newName: "UserClaims");

            migrationBuilder.RenameTable(
                "AspNetRoles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                "AspNetRoleClaims",
                newName: "RoleClaims");

            migrationBuilder.RenameIndex(
                "IX_AspNetUserRoles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                "IX_AspNetUserLogins_UserId",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.RenameIndex(
                "IX_AspNetUserClaims_UserId",
                table: "UserClaims",
                newName: "IX_UserClaims_UserId");

            migrationBuilder.RenameIndex(
                "IX_AspNetRoleClaims_RoleId",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                "PK_UserTokens",
                "UserTokens",
                new[] {"UserId", "LoginProvider", "Name"});

            migrationBuilder.AddPrimaryKey(
                "PK_Users",
                "Users",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_UserRoles",
                "UserRoles",
                new[] {"UserId", "RoleId"});

            migrationBuilder.AddPrimaryKey(
                "PK_UserLogins",
                "UserLogins",
                new[] {"LoginProvider", "ProviderKey"});

            migrationBuilder.AddPrimaryKey(
                "PK_UserClaims",
                "UserClaims",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_Roles",
                "Roles",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_RoleClaims",
                "RoleClaims",
                "Id");

            migrationBuilder.AddForeignKey(
                "FK_RoleClaims_Roles_RoleId",
                "RoleClaims",
                "RoleId",
                "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_UserClaims_Users_UserId",
                "UserClaims",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_UserLogins_Users_UserId",
                "UserLogins",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_UserRoles_Roles_RoleId",
                "UserRoles",
                "RoleId",
                "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_UserRoles_Users_UserId",
                "UserRoles",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_UserTokens_Users_UserId",
                "UserTokens",
                "UserId",
                "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_RoleClaims_Roles_RoleId",
                "RoleClaims");

            migrationBuilder.DropForeignKey(
                "FK_UserClaims_Users_UserId",
                "UserClaims");

            migrationBuilder.DropForeignKey(
                "FK_UserLogins_Users_UserId",
                "UserLogins");

            migrationBuilder.DropForeignKey(
                "FK_UserRoles_Roles_RoleId",
                "UserRoles");

            migrationBuilder.DropForeignKey(
                "FK_UserRoles_Users_UserId",
                "UserRoles");

            migrationBuilder.DropForeignKey(
                "FK_UserTokens_Users_UserId",
                "UserTokens");

            migrationBuilder.DropPrimaryKey(
                "PK_UserTokens",
                "UserTokens");

            migrationBuilder.DropPrimaryKey(
                "PK_Users",
                "Users");

            migrationBuilder.DropPrimaryKey(
                "PK_UserRoles",
                "UserRoles");

            migrationBuilder.DropPrimaryKey(
                "PK_UserLogins",
                "UserLogins");

            migrationBuilder.DropPrimaryKey(
                "PK_UserClaims",
                "UserClaims");

            migrationBuilder.DropPrimaryKey(
                "PK_Roles",
                "Roles");

            migrationBuilder.DropPrimaryKey(
                "PK_RoleClaims",
                "RoleClaims");

            migrationBuilder.RenameTable(
                "UserTokens",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                "Users",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                "UserRoles",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                "UserLogins",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                "UserClaims",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                "Roles",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                "RoleClaims",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameIndex(
                "IX_UserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                "IX_UserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                "IX_UserClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                "IX_RoleClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetUserTokens",
                "AspNetUserTokens",
                new[] {"UserId", "LoginProvider", "Name"});

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetUsers",
                "AspNetUsers",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetUserRoles",
                "AspNetUserRoles",
                new[] {"UserId", "RoleId"});

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetUserLogins",
                "AspNetUserLogins",
                new[] {"LoginProvider", "ProviderKey"});

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetUserClaims",
                "AspNetUserClaims",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetRoles",
                "AspNetRoles",
                "Id");

            migrationBuilder.AddPrimaryKey(
                "PK_AspNetRoleClaims",
                "AspNetRoleClaims",
                "Id");

            migrationBuilder.AddForeignKey(
                "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                "AspNetRoleClaims",
                "RoleId",
                "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserClaims_AspNetUsers_UserId",
                "AspNetUserClaims",
                "UserId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserLogins_AspNetUsers_UserId",
                "AspNetUserLogins",
                "UserId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserRoles_AspNetRoles_RoleId",
                "AspNetUserRoles",
                "RoleId",
                "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserRoles_AspNetUsers_UserId",
                "AspNetUserRoles",
                "UserId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_AspNetUserTokens_AspNetUsers_UserId",
                "AspNetUserTokens",
                "UserId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}