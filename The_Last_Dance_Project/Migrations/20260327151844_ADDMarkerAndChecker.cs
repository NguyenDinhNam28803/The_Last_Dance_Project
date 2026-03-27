using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class ADDMarkerAndChecker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
