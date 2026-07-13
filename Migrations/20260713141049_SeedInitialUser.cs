using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace entity_toy.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PhoneNumber", "Username" },
                values: new object[] { 1, "admin@toy.com", "123456", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
