using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouTubeV2.Application.Migrations
{
    /// <inheritdoc />
    public partial class UserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalance",
                table: "AspNetUsers",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDate",
                table: "AspNetUsers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "AccountBalance", "ConcurrencyStamp", "CreationDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { 0m, "d0a68b9e-243b-49cf-b33b-7b946eca71db", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEN8lpLfxrRkY3X3x+OAjkbsEN7hfQR1X0v5t2fuQNSaIslnfgu980CyYa0pHDsuPTw==", "973ab52b-55fc-45c2-80fa-e54107d588af" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6EBD31DD-0321-4FDA-92FA-CD22A1190DC8",
                columns: new[] { "AccountBalance", "ConcurrencyStamp", "CreationDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { 0m, "68b288dd-58aa-4c4a-8f9f-27110e97451c", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAECAPE+K/CoSs7Jf0SUrg359awex4pVLKw4oun9itkSJwrcJ5uHdx6IHZtujxlZ1F5g==", "5fd85a84-97c7-434c-806b-9c22f5e3e4cb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "CB6A6951-E91A-4A13-B6AC-8634883F5B93",
                columns: new[] { "AccountBalance", "ConcurrencyStamp", "CreationDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { 0m, "d85119b4-a417-46f5-950d-6cea1873f9c6", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEJpRJOUuUIpKr4RTXQ/KF8LvfLMP1o6yv32pTdmKK4mta5sgv+05pFcoJEKa2d2wLw==", "9e83c101-7a02-47a9-8918-a5638bc1ee4a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalance",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "AspNetUsers");

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
        }
    }
}
