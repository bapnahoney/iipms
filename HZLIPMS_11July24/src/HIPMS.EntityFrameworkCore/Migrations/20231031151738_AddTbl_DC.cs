using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTbl_DC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatchClearance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerPlantAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_DispatchClearance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchClearance_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DCData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchClearanceId = table.Column<long>(type: "bigint", nullable: false),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POQty = table.Column<float>(type: "real", nullable: false),
                    DCPreviousQty = table.Column<float>(type: "real", nullable: false),
                    DCBalanceQty = table.Column<float>(type: "real", nullable: false),
                    DCInputQty = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    MaterialClass = table.Column<int>(type: "int", nullable: true),
                    ServiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClientInspectionById = table.Column<long>(type: "bigint", nullable: false),
                    ClientInspectionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractorInspectionById = table.Column<long>(type: "bigint", nullable: false),
                    ContractorInspectionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherInspectionById = table.Column<long>(type: "bigint", nullable: false),
                    OtherInspectionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientInspectionDoneOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractorInspectionDoneOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherInspectionDoneOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rejecter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchCompanyGST = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DCData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DCData_DispatchClearance_DispatchClearanceId",
                        column: x => x.DispatchClearanceId,
                        principalTable: "DispatchClearance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DCData_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DCDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchClearanceId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DCDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DCDocuments_DispatchClearance_DispatchClearanceId",
                        column: x => x.DispatchClearanceId,
                        principalTable: "DispatchClearance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DCData_DispatchClearanceId",
                table: "DCData",
                column: "DispatchClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DCData_POMasterId",
                table: "DCData",
                column: "POMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_DCDocuments_DispatchClearanceId",
                table: "DCDocuments",
                column: "DispatchClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchClearance_POMasterId",
                table: "DispatchClearance",
                column: "POMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DCData");

            migrationBuilder.DropTable(
                name: "DCDocuments");

            migrationBuilder.DropTable(
                name: "DispatchClearance");
        }
    }
}
