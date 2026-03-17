using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlanner.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminSeedingThroughFluentApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a63459c3-04ff-4cd8-bfdd-4687b96eacd4"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("a63459c3-04ff-4cd8-bfdd-4687b96eacd4"), 0, "e86591f2-08fd-4876-90fb-e2a315ef7ef4", null, "admin@gmail.com", true, null, false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAECWa9b6kIIM6w9jh/4UTXIf5PDc2Wv9by7L+2Iz7L8+QK1aFs9AF8gUVwcyUXC/10w==", null, false, "fc10a4bd-acf4-4533-8636-280003beb6a6", false, "admin@gmail.com" });
        }
    }
}
