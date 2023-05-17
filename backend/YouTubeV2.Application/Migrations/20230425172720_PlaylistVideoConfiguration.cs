using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistVideoConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistVideo_Playlists_PlaylistsId",
                table: "PlaylistVideo");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistVideo_Videos_VideosId",
                table: "PlaylistVideo");

            migrationBuilder.RenameColumn(
                name: "VideosId",
                table: "PlaylistVideo",
                newName: "VideoId");

            migrationBuilder.RenameColumn(
                name: "PlaylistsId",
                table: "PlaylistVideo",
                newName: "PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistVideo_VideosId",
                table: "PlaylistVideo",
                newName: "IX_PlaylistVideo_VideoId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "08803007-ce53-4b86-99fc-a6fc4e4d095a", "AQAAAAIAAYagAAAAEA1OwJWOEzSG7SZcMz0hlLdaPpRy3Mx2oRJfXvQINgxe+zy3BCEwmw6nULReKbgowg==", "63061695-9037-4c29-aa6c-13cccda8f5b2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8d3ebc1d-7606-41a5-87e7-c3ab5592c9f2", "AQAAAAIAAYagAAAAEB5smZL6LRrGI5WTsjslHccf1GbxPmxj63Cv4jisSa7ISNAaWaGrJIAWX5ZSsrrb6A==", "11b86b1a-cbff-48ca-852e-867e1c182a80" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a12ff120-4800-40ea-ab24-76f61dddf9ab", "AQAAAAIAAYagAAAAELharXakR+FR6kfmqQauWwUBTL+kK1BI0W6sQjBBxmMzNNKFZ+3iJqYtyZ2DYBCoaQ==", "0b3316b0-21cc-43d4-80e6-facea709efb5" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistVideo_Playlists_PlaylistId",
                table: "PlaylistVideo",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistVideo_Videos_VideoId",
                table: "PlaylistVideo",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistVideo_Playlists_PlaylistId",
                table: "PlaylistVideo");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistVideo_Videos_VideoId",
                table: "PlaylistVideo");

            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "PlaylistVideo",
                newName: "VideosId");

            migrationBuilder.RenameColumn(
                name: "PlaylistId",
                table: "PlaylistVideo",
                newName: "PlaylistsId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistVideo_VideoId",
                table: "PlaylistVideo",
                newName: "IX_PlaylistVideo_VideosId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a290514-864c-4578-b258-5610fb5ba15c", "AQAAAAIAAYagAAAAEJ1SadBj5+/H5lEMnl8xwvF3neCCjjK36ZMdMsl4m2pOCjmQQQloMeYSbGmXi1MnUw==", "f43d5208-1280-4cc5-bdb5-6ebeabd992ab" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad757fe6-7487-41b4-b58e-3754ed1cacb8", "AQAAAAIAAYagAAAAEBClVNEsN4L/rYlhBLY6jhWcOuhErdSsvLr8yxlY91PBrLAbfoOOJZ8nICfCw0QNMw==", "6c71805d-b9b2-4718-9865-16de12544790" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2b70630-e776-437f-b14e-4edb18833145", "AQAAAAIAAYagAAAAEME1mU9yd3uviahPYwP0l8FMtdvLYY2K4t3lVZ1s7EjnmdLVX19kX2/jJdevnwzAdg==", "04252267-18d3-4208-845a-78e6efab4c48" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistVideo_Playlists_PlaylistsId",
                table: "PlaylistVideo",
                column: "PlaylistsId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistVideo_Videos_VideosId",
                table: "PlaylistVideo",
                column: "VideosId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
