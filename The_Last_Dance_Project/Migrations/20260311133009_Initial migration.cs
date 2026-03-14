using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace The_Last_Dance_Project.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDITENTITY_KEY_DEFINITION",
                columns: table => new
                {
                    DEFID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TABLENAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    COLUMNNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DISPLAYNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DISPLAYNAMEEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TABNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TABNAMEEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISCOMPARE = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    DATATYPE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LOOKUPTABLE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDITENTITY_KEY_DEFINITION", x => x.DEFID);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RecordStatus = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignPaths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvestorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidentCountryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FATCA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustodyCd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStaff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectDes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangeBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproveBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustId);
                });

            migrationBuilder.CreateTable(
                name: "MTTRAN",
                columns: table => new
                {
                    MTTRANID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OBJCHANGE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BUSDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ACTIONDATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MODCODE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KEYFIELD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KEYVALUE = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KEYFIELDEXT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KEYVALUEEXT = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MTLSTATUS = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    MTLTYPE = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MAKER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MAIPADDRESS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MAWSNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CHECKER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CHKIPADDRESS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CHKWSNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MTTRAN", x => x.MTTRANID);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEMCODE",
                columns: table => new
                {
                    SYSTEMCODEID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ISACTIVE = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEMCODE", x => x.SYSTEMCODEID);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_CONTACT",
                columns: table => new
                {
                    ContactId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    InfoType = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FaxAttention = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefault = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerCustId = table.Column<string>(type: "nvarchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_CONTACT", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_CONTACT_Customers_CustId",
                        column: x => x.CustId,
                        principalTable: "Customers",
                        principalColumn: "CustId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_CONTACT_Customers_CustomerCustId",
                        column: x => x.CustomerCustId,
                        principalTable: "Customers",
                        principalColumn: "CustId");
                });

            migrationBuilder.CreateTable(
                name: "AUDITENTITY_KEY",
                columns: table => new
                {
                    MTTRANFLDID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MTTRANID = table.Column<long>(type: "bigint", nullable: false),
                    COLUMNNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DISPLAYNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DISPLAYNAMEEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OLDVAL = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NEWVAL = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DISPLAYVALOLD = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DISPLAYVALNEW = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    KEYMAP = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KEYMAPEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TABLENAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TABNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TABNAMEEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OLDVALEN = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NEWVALEN = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AuditentityMtTranId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDITENTITY_KEY", x => x.MTTRANFLDID);
                    table.ForeignKey(
                        name: "FK_AUDITENTITY_KEY_MTTRAN_AuditentityMtTranId",
                        column: x => x.AuditentityMtTranId,
                        principalTable: "MTTRAN",
                        principalColumn: "MTTRANID");
                    table.ForeignKey(
                        name: "FK_AUDITENTITY_KEY_MTTRAN_MTTRANID",
                        column: x => x.MTTRANID,
                        principalTable: "MTTRAN",
                        principalColumn: "MTTRANID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEMCODE_VALUE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SYSTEMCODEID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODEVALUE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DISPLAYVALUE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DISPLAYVALUEEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ORDERBY = table.Column<int>(type: "int", nullable: true),
                    ISDEFAULT = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEMCODE_VALUE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SYSTEMCODE_VALUE_SYSTEMCODE_SYSTEMCODEID",
                        column: x => x.SYSTEMCODEID,
                        principalTable: "SYSTEMCODE",
                        principalColumn: "SYSTEMCODEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDITENTITY_KEY_AuditentityMtTranId",
                table: "AUDITENTITY_KEY",
                column: "AuditentityMtTranId");

            migrationBuilder.CreateIndex(
                name: "IX_AUDITENTITY_KEY_MTTRANID",
                table: "AUDITENTITY_KEY",
                column: "MTTRANID");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_CONTACT_CustId",
                table: "CUSTOMER_CONTACT",
                column: "CustId");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_CONTACT_CustomerCustId",
                table: "CUSTOMER_CONTACT",
                column: "CustomerCustId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEMCODE_VALUE_SYSTEMCODEID",
                table: "SYSTEMCODE_VALUE",
                column: "SYSTEMCODEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDITENTITY_KEY");

            migrationBuilder.DropTable(
                name: "AUDITENTITY_KEY_DEFINITION");

            migrationBuilder.DropTable(
                name: "CUSTOMER_CONTACT");

            migrationBuilder.DropTable(
                name: "SYSTEMCODE_VALUE");

            migrationBuilder.DropTable(
                name: "MTTRAN");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "SYSTEMCODE");
        }
    }
}
