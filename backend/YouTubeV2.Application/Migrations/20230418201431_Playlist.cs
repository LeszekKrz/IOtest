using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Playlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "Videos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "053c62ae-abc9-4187-8cc5-d0f89ffb0169", "AQAAAAIAAYagAAAAEKUzTh2bu3LOWRdDtIc+d6/4LmZxcKKN1yQOs1XBFHE/n0mz2/lbiEfG9G/Fq4YJ0w==", "a964067e-fbc4-4eb1-bd32-8ee5e9b33d23" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2585c6d-3063-4d91-b8b6-d1d7d55e76b8", "AQAAAAIAAYagAAAAEEqhMpw4PEdBhya26Qj7Q0AMlPUTxdwlqCbtE94nXyai3hjPpubVzycWo02WLq0HTQ==", "306748b5-cc0b-475c-ad65-f07df42457a9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "20854744-2310-46cf-8767-e51f9d19ea81", "AQAAAAIAAYagAAAAEPaIwHM8/gxbmaMi8yaJuYK9Do+kfjQmc47/d37281VgBr0I2+jgi62MUJkMjz2hnQ==", "ac2be167-165b-4ea6-91ae-7e5dd3ef3429" });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_CreatorId",
                table: "Playlists",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Videos");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b502f63-4717-4180-a782-998d9ccd3c2b", "AQAAAAIAAYagAAAAENfL3ShmDUXWqUSJSbnzkc2RMhtdW+b0Hinx/rObc3A43PIOGx6RLGR8MT5IkPPquw==", "44d91bb0-39cd-4b82-a77b-67afd2942474" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f39285f5-abae-414e-a484-a45cdf66188a", "AQAAAAIAAYagAAAAEA+L9ASrjRPxC4j3nSqjCXv1z7hqVNv+JX5rJJXW5Sm9EVvIrtjsSYPQrD1xBXpbvg==", "9bcb5a4b-38f9-4bf6-9730-fe09bccc11f0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3eed7e77-5dbc-427d-898f-1f4184cebdde", "AQAAAAIAAYagAAAAEJiZvxw0R8PLyPTaYnIksvBzgxR0hW38Bk/2STl466hk9a1m1i/XhLlOPPP42YNDVA==", "ab902e25-0d69-4c46-9ce1-d8d72312ee52" });
        }
    }
}
