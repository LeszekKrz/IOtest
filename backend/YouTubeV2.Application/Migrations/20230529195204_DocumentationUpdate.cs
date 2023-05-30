using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class DocumentationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetType",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "Tickets");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "70bd0541-901a-446a-ba1f-e6acfb22ae48", "AQAAAAIAAYagAAAAEBzFNRxPseQJzczP1xLw0QKCJDGfTzTHTeb9qfw5JzPCTZwKS8xwnUT1uEw0ADNRXA==", "644d86b9-10bd-43ce-b90c-2814240023cc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5bc00c0b-fd0f-448d-b1e5-e22890678f8f", "AQAAAAIAAYagAAAAECTdQQODiNrk8lL4WG+KoIh38+1qH2ceHBq+Q8b7J9mw43NHbm7ES9VXCEH96VF5Yw==", "11d2195a-5927-4203-8b8f-15c85ed50e2f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f8b998e-2db4-4e02-b524-64b4e6f553a3", "AQAAAAIAAYagAAAAEGWaCt7fyDUuPdtJpxt1rWH2bj7mXdrDZcOUclmS3u/qWhvMdGPXcXhUgBaU9Z2VBQ==", "ef840399-8c52-4481-8a6c-52ed7115085b" });
        }
    }
}
