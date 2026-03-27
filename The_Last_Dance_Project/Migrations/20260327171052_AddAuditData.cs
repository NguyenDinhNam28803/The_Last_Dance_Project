using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AUDITENTITY_KEY_DEFINITION",
                columns: new[] { "DEFID", "COLUMNNAME", "DATATYPE", "DESCRIPTION", "DISPLAYNAME", "DISPLAYNAMEEN", "ISCOMPARE", "LOOKUPTABLE", "TABNAME", "TABNAMEEN", "TABLENAME" },
                values: new object[,]
                {
                    { 1, "NAME", "VARCHAR2", null, "Tên", null, "Y", null, null, null, "CUSTOMER" },
                    { 2, "EMAIL", "VARCHAR2", null, "Email", null, "Y", null, null, null, "CUSTOMER" },
                    { 3, "STATUS", "VARCHAR2", null, "Trạng thái", null, "Y", null, null, null, "ACCOUNT" }
                });

            migrationBuilder.InsertData(
                table: "MTTRAN",
                columns: new[] { "MTTRANID", "ACTIONDATE", "BUSDATE", "CHECKER", "CHKIPADDRESS", "CHKWSNAME", "DESCRIPTION", "KEYFIELD", "KEYFIELDEXT", "KEYVALUE", "KEYVALUEEXT", "MAIPADDRESS", "MAWSNAME", "MAKER", "MODCODE", "MTLSTATUS", "MTLTYPE", "OBJCHANGE" },
                values: new object[,]
                {
                    { 1001L, new DateTime(2026, 3, 27, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Tạo tài khoản maker_system", "CustId", null, "ACC_MAKER_001", null, null, null, "maker_system", "CUST", "A", "A", "Customer" },
                    { 1002L, new DateTime(2026, 3, 27, 12, 5, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Tạo tài khoản", "AccId", null, "ACC_0001", null, null, null, "maker_system", "ACC", "A", "C", "Account" }
                });

            migrationBuilder.InsertData(
                table: "AUDITENTITY_KEY",
                columns: new[] { "MTTRANFLDID", "AuditentityMtTranId", "COLUMNNAME", "DESCRIPTION", "DISPLAYNAME", "DISPLAYNAMEEN", "DISPLAYVALNEW", "DISPLAYVALOLD", "KEYMAP", "KEYMAPEN", "MTTRANID", "NEWVAL", "NEWVALEN", "OLDVAL", "OLDVALEN", "TABNAME", "TABNAMEEN", "TABLENAME" },
                values: new object[,]
                {
                    { 2001L, null, "NAME", null, "Tên", null, null, null, null, null, 1001L, "Maker System Account", null, null, null, "Thông tin khách hàng", null, "CUSTOMER" },
                    { 2002L, null, "EMAIL", null, "Email", null, null, null, null, null, 1001L, "maker_sys@test.com", null, null, null, "Thông tin khách hàng", null, "CUSTOMER" },
                    { 2003L, null, "STATUS", null, "Trạng thái", null, null, null, null, null, 1002L, "Active", null, null, null, "Thông tin tài khoản", null, "ACCOUNT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY",
                keyColumn: "MTTRANFLDID",
                keyValue: 2001L);

            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY",
                keyColumn: "MTTRANFLDID",
                keyValue: 2002L);

            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY",
                keyColumn: "MTTRANFLDID",
                keyValue: 2003L);

            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY_DEFINITION",
                keyColumn: "DEFID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY_DEFINITION",
                keyColumn: "DEFID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AUDITENTITY_KEY_DEFINITION",
                keyColumn: "DEFID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MTTRAN",
                keyColumn: "MTTRANID",
                keyValue: 1001L);

            migrationBuilder.DeleteData(
                table: "MTTRAN",
                keyColumn: "MTTRANID",
                keyValue: 1002L);
        }
    }
}
