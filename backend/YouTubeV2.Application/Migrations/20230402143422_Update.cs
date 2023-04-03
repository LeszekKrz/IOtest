using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b2b45fb-8321-40d6-9487-c5b593698fa9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7aa01131-19ba-4154-b9a2-439c7031ba3b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa381995-8e47-4b1f-83f1-b534e6f41aa4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "24de6e76-9a75-490b-bddc-a66e91e20fa4", null, "Administrator", "ADMINISTRATOR" },
                    { "37f0c423-8d1c-4817-a1af-e1bf60c9a0eb", null, "Simple", "SIMPLE" },
                    { "87978f1a-4281-4a59-8ee8-52b1ed4b3d56", null, "Creator", "CREATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "24de6e76-9a75-490b-bddc-a66e91e20fa4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37f0c423-8d1c-4817-a1af-e1bf60c9a0eb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87978f1a-4281-4a59-8ee8-52b1ed4b3d56");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3b2b45fb-8321-40d6-9487-c5b593698fa9", null, "Simple", "SIMPLE" },
                    { "7aa01131-19ba-4154-b9a2-439c7031ba3b", null, "Administrator", "ADMINISTRATOR" },
                    { "fa381995-8e47-4b1f-83f1-b534e6f41aa4", null, "Creator", "CREATOR" }
                });
        }
    }
}
