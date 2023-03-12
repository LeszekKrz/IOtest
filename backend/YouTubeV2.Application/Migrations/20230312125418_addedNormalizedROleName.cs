using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class addedNormalizedROleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "468a3f2a-a631-4afe-88bf-e8010494e937");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a21a17b-e8e6-4ee9-a951-6e8d1b8c073f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ba17fb3-7b1b-4033-bf09-1be3b4950e6a");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "468a3f2a-a631-4afe-88bf-e8010494e937", null, "CREATOR", null },
                    { "8a21a17b-e8e6-4ee9-a951-6e8d1b8c073f", null, "USER", null },
                    { "9ba17fb3-7b1b-4033-bf09-1be3b4950e6a", null, "ADMIN", null }
                });
        }
    }
}
