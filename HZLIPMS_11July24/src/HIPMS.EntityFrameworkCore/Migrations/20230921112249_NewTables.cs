using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class NewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "POMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Plant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionClearance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    SyncStatus = table.Column<int>(type: "int", nullable: false),
                    SyncOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncCount = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionClearance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionClearance_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POQty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialClass = table.Column<int>(type: "int", nullable: false),
                    ServiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POItems_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ICData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionClearanceId = table.Column<long>(type: "bigint", nullable: false),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POQty = table.Column<float>(type: "real", nullable: false),
                    ICPreviousQty = table.Column<float>(type: "real", nullable: false),
                    ICBalanceQty = table.Column<float>(type: "real", nullable: false),
                    ICInputQty = table.Column<float>(type: "real", nullable: false),
                    InspectionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Material_Class = table.Column<int>(type: "int", nullable: true),
                    ServiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionDoneOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ICData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ICData_InspectionClearance_InspectionClearanceId",
                        column: x => x.InspectionClearanceId,
                        principalTable: "InspectionClearance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ICData_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ICDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionClearanceId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ICDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ICDocuments_InspectionClearance_InspectionClearanceId",
                        column: x => x.InspectionClearanceId,
                        principalTable: "InspectionClearance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ICData_InspectionClearanceId",
                table: "ICData",
                column: "InspectionClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ICData_POMasterId",
                table: "ICData",
                column: "POMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ICDocuments_InspectionClearanceId",
                table: "ICDocuments",
                column: "InspectionClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionClearance_POMasterId",
                table: "InspectionClearance",
                column: "POMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_POItems_POMasterId",
                table: "POItems",
                column: "POMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ICData");

            migrationBuilder.DropTable(
                name: "ICDocuments");

            migrationBuilder.DropTable(
                name: "POItems");

            migrationBuilder.DropTable(
                name: "InspectionClearance");

            migrationBuilder.DropTable(
                name: "POMaster");
        }
    }
}
