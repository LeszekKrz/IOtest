using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Reactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reactions_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2cb2ab0-4414-47cb-8333-b4069be7eafe", "AQAAAAIAAYagAAAAELhloF/2e4EFnRjF5YNiJjPqNx6d/9eSjf3C2++ReUi+5LYlK1nfWqRsJYOiFYZuvw==", "945b000e-3a5e-49d1-b2a6-c1a8d942151e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29f1ab44-3fe7-4a59-add3-f7028835a3e1", "AQAAAAIAAYagAAAAEDKl+VaKx4KGNXKG4uMAGjFP/R16LI89iASPL447UYhS+ekyuBPG7sgHkL1LRARkjg==", "9ac6d502-43e1-4299-a722-bdb6ca52cb08" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f37ec74c-116f-4460-914a-96ba6e62643e", "AQAAAAIAAYagAAAAEG+F61oBTW3mXaNVwf5kPEznqHr/SBvbtvBOXVhiiP80GHmCQSi+VaZwiUmfEIcAGA==", "a67f391f-89e3-4158-aeff-47dd086d1993" });

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId",
                table: "Reactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_VideoId",
                table: "Reactions",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reactions");

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
        }
    }
}
