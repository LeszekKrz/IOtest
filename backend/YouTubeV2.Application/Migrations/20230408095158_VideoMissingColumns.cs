using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class VideoMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EditDate",
                table: "Videos",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ProcessingProgress",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UploadDate",
                table: "Videos",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04b4fe2b-5085-4b7a-819b-23d0a04550c6", null, "Simple", "SIMPLE" },
                    { "8b929466-a7c3-45be-a34d-b500623b5be0", null, "Creator", "CREATOR" },
                    { "e5cc3ad4-1565-473c-89da-f6b3f70dac69", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04b4fe2b-5085-4b7a-819b-23d0a04550c6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8b929466-a7c3-45be-a34d-b500623b5be0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5cc3ad4-1565-473c-89da-f6b3f70dac69");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "EditDate",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ProcessingProgress",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Videos");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4b366327-19d1-4d54-b595-ba02e10b8afa", null, "Simple", "SIMPLE" },
                    { "7ca775ff-cffe-42ae-85ad-cf13e9f39975", null, "Creator", "CREATOR" },
                    { "ed0a3367-f6ee-437d-856e-eecf86267c02", null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
