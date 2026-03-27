using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[] { "USER", "User account", "User" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "USER");
        }
    }
}
