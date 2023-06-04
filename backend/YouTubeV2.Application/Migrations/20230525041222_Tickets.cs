using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class Tickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmitterId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "40a0f598-fe79-4bb2-9f0b-20c4df9ffacf", "AQAAAAIAAYagAAAAEIdEyKdZqrln9DIONQDgwdc7wGdkOY/0tEyaKdY1jp+9hH1p41P1qQvSEfPTqIGwSg==", "a3c59396-79fa-451d-8bd1-7ad5bf6d3d93" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8b2ff47-c4bc-4006-8c21-a2a959668e2d", "AQAAAAIAAYagAAAAEOIunq+KYgvoaSfrKFO772i3rF8FTrnl/psgguwyERDdeYBQAZ9iU06OGSs7kQxGmQ==", "a1c9318f-2d76-48ed-b7bf-a7b1aca6f629" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a19c0873-7747-4c85-b91f-e29435ba34fb", "AQAAAAIAAYagAAAAENZisR1gevzZ2ljN3QgOc526D0WOpJ4akIRtz5sMQ9FNW5uX/wJGhli41HAu5YquDw==", "e2b242a9-35db-4677-b0f0-554f43afe399" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SubmitterId",
                table: "Tickets",
                column: "SubmitterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d0a68b9e-243b-49cf-b33b-7b946eca71db", "AQAAAAIAAYagAAAAEN8lpLfxrRkY3X3x+OAjkbsEN7hfQR1X0v5t2fuQNSaIslnfgu980CyYa0pHDsuPTw==", "973ab52b-55fc-45c2-80fa-e54107d588af" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "68b288dd-58aa-4c4a-8f9f-27110e97451c", "AQAAAAIAAYagAAAAECAPE+K/CoSs7Jf0SUrg359awex4pVLKw4oun9itkSJwrcJ5uHdx6IHZtujxlZ1F5g==", "5fd85a84-97c7-434c-806b-9c22f5e3e4cb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d85119b4-a417-46f5-950d-6cea1873f9c6", "AQAAAAIAAYagAAAAEJpRJOUuUIpKr4RTXQ/KF8LvfLMP1o6yv32pTdmKK4mta5sgv+05pFcoJEKa2d2wLw==", "9e83c101-7a02-47a9-8918-a5638bc1ee4a" });
        }
    }
}
