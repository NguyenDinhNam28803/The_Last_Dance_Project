using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleId1",
                table: "Customers",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.RoleId);
                });

            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { "ADMIN", "System administrator with full permissions", "Administrator" },
                    { "MANAGER", "User with managerial permissions", "Manager" },
                    { "USER", "Default application user", "User" }
                });

            migrationBuilder.InsertData(
                table: "SYSTEMCODE",
                columns: new[] { "SYSTEMCODEID", "DESCRIPTION", "ISACTIVE", "NAME" },
                values: new object[] { "ROLE", "Application roles", "Y", "Role definitions" });

            migrationBuilder.InsertData(
                table: "SYSTEMCODE_VALUE",
                columns: new[] { "ID", "CODEVALUE", "DISPLAYVALUE", "DISPLAYVALUEEN", "ISDEFAULT", "ORDERBY", "SYSTEMCODEID" },
                values: new object[,]
                {
                    { 1001, "ADMIN", "Administrator", "Administrator", "N", 1, "ROLE" },
                    { 1002, "USER", "User", "User", "Y", 2, "ROLE" },
                    { 1003, "MANAGER", "Manager", "Manager", "N", 3, "ROLE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RoleId",
                table: "Customers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RoleId1",
                table: "Customers",
                column: "RoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ROLE_RoleId",
                table: "Customers",
                column: "RoleId",
                principalTable: "ROLE",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ROLE_RoleId1",
                table: "Customers",
                column: "RoleId1",
                principalTable: "ROLE",
                principalColumn: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ROLE_RoleId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ROLE_RoleId1",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "ROLE");

            migrationBuilder.DropIndex(
                name: "IX_Customers_RoleId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_RoleId1",
                table: "Customers");

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE_VALUE",
                keyColumn: "ID",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE_VALUE",
                keyColumn: "ID",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE_VALUE",
                keyColumn: "ID",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE",
                keyColumn: "SYSTEMCODEID",
                keyValue: "ROLE");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Customers");
        }
    }
}
