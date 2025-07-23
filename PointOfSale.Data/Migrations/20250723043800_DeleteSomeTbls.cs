using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointOfSale.Data.Migrations
{
    public partial class DeleteSomeTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolMenus_Rols_IdRolNavigationIdRol",
                table: "RolMenus");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_ApplicationUserId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Users_IdUsersNavigationIdUsers",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rols");

            migrationBuilder.DropIndex(
                name: "IX_Sales_ApplicationUserId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_IdUsersNavigationIdUsers",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_RolMenus_IdRolNavigationIdRol",
                table: "RolMenus");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IdUsersNavigationIdUsers",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IdRolNavigationIdRol",
                table: "RolMenus");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsers",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_IdUsers",
                table: "Sales",
                column: "IdUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_IdUsers",
                table: "Sales",
                column: "IdUsers",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_IdUsers",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_IdUsers",
                table: "Sales");

            migrationBuilder.AlterColumn<int>(
                name: "IdUsers",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsersNavigationIdUsers",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRolNavigationIdRol",
                table: "RolMenus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rols",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rols", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUsers = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRolNavigationIdRol = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdRol = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUsers);
                    table.ForeignKey(
                        name: "FK_Users_Rols_IdRolNavigationIdRol",
                        column: x => x.IdRolNavigationIdRol,
                        principalTable: "Rols",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ApplicationUserId",
                table: "Sales",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_IdUsersNavigationIdUsers",
                table: "Sales",
                column: "IdUsersNavigationIdUsers");

            migrationBuilder.CreateIndex(
                name: "IX_RolMenus_IdRolNavigationIdRol",
                table: "RolMenus",
                column: "IdRolNavigationIdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdRolNavigationIdRol",
                table: "Users",
                column: "IdRolNavigationIdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_RolMenus_Rols_IdRolNavigationIdRol",
                table: "RolMenus",
                column: "IdRolNavigationIdRol",
                principalTable: "Rols",
                principalColumn: "IdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_ApplicationUserId",
                table: "Sales",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Users_IdUsersNavigationIdUsers",
                table: "Sales",
                column: "IdUsersNavigationIdUsers",
                principalTable: "Users",
                principalColumn: "IdUsers");
        }
    }
}
