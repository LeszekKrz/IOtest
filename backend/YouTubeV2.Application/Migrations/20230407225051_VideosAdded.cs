using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class VideosAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4b366327-19d1-4d54-b595-ba02e10b8afa", null, "Simple", "SIMPLE" },
                    { "7ca775ff-cffe-42ae-85ad-cf13e9f39975", null, "Creator", "CREATOR" },
                    { "ed0a3367-f6ee-437d-856e-eecf86267c02", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_VideoId",
                table: "Tags",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_UserId",
                table: "Videos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b366327-19d1-4d54-b595-ba02e10b8afa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ca775ff-cffe-42ae-85ad-cf13e9f39975");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed0a3367-f6ee-437d-856e-eecf86267c02");

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
    }
}
