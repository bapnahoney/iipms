using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTbl_NCR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NCRMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discipline = table.Column<int>(type: "int", nullable: false),
                    NCRNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NCRDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateRaised = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Criticality = table.Column<int>(type: "int", nullable: false),
                    VendorComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NCRMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NCRMaster_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NCRDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NCRMasterId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NCRDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NCRDocuments_NCRMaster_NCRMasterId",
                        column: x => x.NCRMasterId,
                        principalTable: "NCRMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NCRDocuments_NCRMasterId",
                table: "NCRDocuments",
                column: "NCRMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_NCRMaster_POMasterId",
                table: "NCRMaster",
                column: "POMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NCRDocuments");

            migrationBuilder.DropTable(
                name: "NCRMaster");
        }
    }
}
