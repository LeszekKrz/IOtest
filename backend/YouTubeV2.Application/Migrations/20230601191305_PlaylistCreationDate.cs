using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistCreationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDate",
                table: "Playlists",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f028aa56-788e-4d1d-8d40-bb4a8bba4506", "AQAAAAIAAYagAAAAEM88tJGp+5UvSvFkPVedTVQiZwR5rvqV2+1bmVRYgMWDnGminWx8k1eZhClxpGiE8Q==", "74c30ccd-2a89-4c22-b360-571832d88583" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "846e67d7-355d-4762-b965-fb14352a34a5", "AQAAAAIAAYagAAAAEDR17yhGGdGBDXPhDxKTesmwNtLjF71MLUPsvYRxfH5TA6hRkOlSg7fDctlmFnv8dw==", "4e5bd490-54f2-475d-9e57-37a5961e3a12" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "172efd08-b6aa-498f-bd43-443769aa20b6", "AQAAAAIAAYagAAAAEC0YMo1cNxdXcm2LyuUi1mNayjGH0eIwfydodQNfwQFPEYiAHyAbtLmYidA0We0XzQ==", "a1cc58b3-d5f8-47ee-ab24-68fef43f7531" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Playlists");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34cda522-d0ff-4128-879a-b13e7271e309", "AQAAAAIAAYagAAAAEDo822y7l2TnoKWTtu1DhICefgahBOJotXUUsb0SHt0ms+2jni1dcr9Y44hwvzDLdg==", "698bd4a1-715d-40c1-b9fb-e71b6a5e184e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "21e0d68c-90b3-4d94-8aa0-2e2fa85c0010", "AQAAAAIAAYagAAAAEBp2j5xp06zrpAxXGyzcUBJpVZX9DgqkyCfx4rD0Y0r2JIhfclSaNO/ZYYnpuoK1hQ==", "005d2e45-f726-454e-8ce1-568282102840" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aaf973fd-992b-45a4-8e2e-0a09a694d761", "AQAAAAIAAYagAAAAEM5iuI7oVcaMfUA0+fc0e01aVQdMSieMjuunWjVO7Wv2zh0P52OjeyfUq1yMc6P6pw==", "7d9e742a-45b5-45eb-9b6f-b703805a8a58" });
        }
    }
}
