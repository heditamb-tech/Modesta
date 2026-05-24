using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modesta.Migrations
{
    /// <inheritdoc />
    public partial class AdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Bio", "CreatedAt", "Email", "FirstName", "IsAdmin", "LastName", "PasswordHash", "PrivacySetting", "ProfilePicturePath", "UITheme", "Username" },
                values: new object[] { 1, null, new DateTime(2026, 5, 24, 21, 10, 23, 510, DateTimeKind.Local).AddTicks(8812), "admin@modesta.be", null, true, null, "$2a$11$GemHY66tnzn1vNmIg6HGN.yqqawnns6IKfh/.SjL86qyAGsNhgJ0i", "private", null, "beige", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
