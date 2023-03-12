using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class removeIDFromEntityConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "690af15e-6e0c-4a68-b68c-d8ed4461eda7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a57987a1-8a84-4bf5-8766-757b8fed016c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5e3469f8-224e-430d-bbcf-e910162e9864", null, "USER", "USER" },
                    { "a185aaa7-9aad-4dad-b5f3-bd83d5e23faa", null, "ADMIN", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e3469f8-224e-430d-bbcf-e910162e9864");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a185aaa7-9aad-4dad-b5f3-bd83d5e23faa");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "690af15e-6e0c-4a68-b68c-d8ed4461eda7", null, "ADMIN", "ADMIN" },
                    { "a57987a1-8a84-4bf5-8766-757b8fed016c", null, "USER", "USER" }
                });
        }
    }
}
