using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class MakeTicketResponseNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
        }
    }
}
