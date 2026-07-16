using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GMMS.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordAndSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Tbl_User",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.InsertData(
                table: "Tbl_User",
                columns: new[] { "UserId", "CreatedAt", "CreatedBy", "IsActive", "MustChangePassword", "PasswordHash", "Role", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, true, "$2a$11$51n3uUKJ0zfp8Suf/AH2wulPNfkwy4CqEslohD8VpYIwA5gTaOXKG", "Owner", null, null, "owner" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, true, "$2a$11$GJjdLlC9Kb9d4LGvnYVHO.IiBoNPUTRCDS4.TR92T7bHxTveFHtkq", "Admin", null, null, "admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tbl_User",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tbl_User",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Tbl_User");
        }
    }
}
