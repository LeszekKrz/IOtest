using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Video : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "PlaylistVideo",
                columns: table => new
                {
                    PlaylistsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VideosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistVideo", x => new { x.PlaylistsId, x.VideosId });
                    table.ForeignKey(
                        name: "FK_PlaylistVideo_Playlists_PlaylistsId",
                        column: x => x.PlaylistsId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistVideo_Videos_VideosId",
                        column: x => x.VideosId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2646084-9389-4cf9-8e19-81aec9de830d", "AQAAAAIAAYagAAAAEMAR/AjZLBwz5X0oXszaD2U2NDgC8B1+AhjWlv6h9cQwLlXiGV8UpzVItgWva6iz4A==", "c70f5b0b-1e11-4199-8af5-346171e6edf1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d3ad30ea-72df-4731-9b39-6e7911adb86b", "AQAAAAIAAYagAAAAEObhouxUCAqVuilLVJ9T8fErDAb0FYVE1iaTnNJyTEkwLWhWqe3Z2LqjIy980o6tGA==", "cc039acb-3d3b-4149-844b-2f5a3f85609f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "811e4ad8-a263-4d7f-bfa5-1111bc1d975b", "AQAAAAIAAYagAAAAEIIdXjRgOnpo4A8SBZZ3X350Y+y7g/D4wfEr143VxwiezRU+6V2CzcHWBfiY3XzfHQ==", "bc0b9e79-66e5-42ef-bb0e-e837aa93fc3c" });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistVideo_VideosId",
                table: "PlaylistVideo",
                column: "VideosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistVideo");

            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "Videos",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }
    }
}
