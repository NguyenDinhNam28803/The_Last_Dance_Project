using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class CheckRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CUSTOMER_CONTACT_Customers_CustomerCustId",
                table: "CUSTOMER_CONTACT");

            migrationBuilder.DropIndex(
                name: "IX_CUSTOMER_CONTACT_CustomerCustId",
                table: "CUSTOMER_CONTACT");

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "ADMIN");

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "MANAGER");

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "USER");

            migrationBuilder.DropColumn(
                name: "CustomerCustId",
                table: "CUSTOMER_CONTACT");

            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { "CHECKER", "Approve or reject requests", "Checker" },
                    { "MAKER", "Create and modify requests", "Maker" }
                });

            migrationBuilder.InsertData(
                table: "SYSTEMCODE",
                columns: new[] { "SYSTEMCODEID", "DESCRIPTION", "ISACTIVE", "NAME" },
                values: new object[] { "STATUS", "Statuses for MTTRAN", "Y", "Transaction Status" });

            migrationBuilder.InsertData(
                table: "SYSTEMCODE_VALUE",
                columns: new[] { "ID", "CODEVALUE", "DISPLAYVALUE", "DISPLAYVALUEEN", "ISDEFAULT", "ORDERBY", "SYSTEMCODEID" },
                values: new object[,]
                {
                    { 1004, "MAKER", "Maker", "Maker", "N", 4, "ROLE" },
                    { 1005, "CHECKER", "Checker", "Checker", "N", 5, "ROLE" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustId", "ApproveBy", "ApproveDate", "CloseDate", "CreatedBy", "CreatedDate", "CustodyCd", "DateOfBirth", "Email", "FATCA", "Gender", "InstitutionTypeId", "InvestorCode", "IsStaff", "LastChangeBy", "LastChangeDate", "Name", "NameOther", "Nationality", "OpenDate", "OpenVia", "PasswordHash", "PhoneNumber", "PlaceOfBirth", "RecordStatus", "RegistrationType", "RejectDes", "ResidentCountryId", "RoleId", "RoleId1", "ShortName", "SignPaths", "Status", "UserName" },
                values: new object[,]
                {
                    { "ACC_CHECKER_001", null, null, null, null, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "checker_sys@test.com", null, null, null, null, null, null, null, "Checker System Account", null, null, null, null, "$2a$11$qR7i5J2k5W.O7pL7X/f8/.hHqXmZ.xH1v5B8j0C9iE8A1D1G1I1K1", null, null, "1", null, null, null, "CHECKER", null, null, null, "Active", "checker_system" },
                    { "ACC_MAKER_001", null, null, null, null, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "maker_sys@test.com", null, null, null, null, null, null, null, "Maker System Account", null, null, null, null, "$2a$11$qR7i5J2k5W.O7pL7X/f8/.hHqXmZ.xH1v5B8j0C9iE8A1D1G1I1K1", null, null, "1", null, null, null, "MAKER", null, null, null, "Active", "maker_system" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustId",
                keyValue: "ACC_CHECKER_001");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustId",
                keyValue: "ACC_MAKER_001");

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE",
                keyColumn: "SYSTEMCODEID",
                keyValue: "STATUS");

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE_VALUE",
                keyColumn: "ID",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "SYSTEMCODE_VALUE",
                keyColumn: "ID",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "CHECKER");

            migrationBuilder.DeleteData(
                table: "ROLE",
                keyColumn: "RoleId",
                keyValue: "MAKER");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCustId",
                table: "CUSTOMER_CONTACT",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ROLE",
                columns: new[] { "RoleId", "Description", "Name" },
                values: new object[,]
                {
                    { "ADMIN", "System administrator with full permissions", "Administrator" },
                    { "MANAGER", "User with managerial permissions", "Manager" },
                    { "USER", "Default application user", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_CONTACT_CustomerCustId",
                table: "CUSTOMER_CONTACT",
                column: "CustomerCustId");

            migrationBuilder.AddForeignKey(
                name: "FK_CUSTOMER_CONTACT_Customers_CustomerCustId",
                table: "CUSTOMER_CONTACT",
                column: "CustomerCustId",
                principalTable: "Customers",
                principalColumn: "CustId");
        }
    }
}
