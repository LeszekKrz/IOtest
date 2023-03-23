using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Addednewroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a9fa4c99-8b0b-4163-96a8-a427888d9ab5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f6a4d4f8-d3ea-4cc2-8fb1-48d4e81465b5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f70f713f-6b7f-4940-8545-43a65671a8c3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "44b04ab7-e63c-4979-99eb-c8bef14630d0", null, "Simple", "SIMPLE" },
                    { "51b0c472-5583-49e2-882e-f21e9234381e", null, "Administrator", "ADMINISTRATOR" },
                    { "f70541fd-4c68-4490-a6b8-7c616c1cd368", null, "Creator", "CREATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44b04ab7-e63c-4979-99eb-c8bef14630d0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51b0c472-5583-49e2-882e-f21e9234381e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f70541fd-4c68-4490-a6b8-7c616c1cd368");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a9fa4c99-8b0b-4163-96a8-a427888d9ab5", null, "CREATOR", "CREATOR" },
                    { "f6a4d4f8-d3ea-4cc2-8fb1-48d4e81465b5", null, "ADMIN", "ADMIN" },
                    { "f70f713f-6b7f-4940-8545-43a65671a8c3", null, "USER", "USER" }
                });
        }
    }
}
