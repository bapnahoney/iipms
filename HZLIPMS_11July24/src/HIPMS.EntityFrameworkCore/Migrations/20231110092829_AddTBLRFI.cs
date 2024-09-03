using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class AddTBLRFI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RFI",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RFI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RFI_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RFIData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RFIId = table.Column<long>(type: "bigint", nullable: false),
                    POMasterId = table.Column<long>(type: "bigint", nullable: false),
                    PONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POQty = table.Column<float>(type: "real", nullable: false),
                    PreviousQty = table.Column<float>(type: "real", nullable: false),
                    BalanceQty = table.Column<float>(type: "real", nullable: false),
                    InputQty = table.Column<float>(type: "real", nullable: false),
                    InspectionById = table.Column<long>(type: "bigint", nullable: false),
                    InspectionBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    MaterialClass = table.Column<int>(type: "int", nullable: true),
                    ServiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Approver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rejecter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialClassValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionDoneOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFIData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RFIData_POMaster_POMasterId",
                        column: x => x.POMasterId,
                        principalTable: "POMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RFIData_RFI_RFIId",
                        column: x => x.RFIId,
                        principalTable: "RFI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RFIDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RFIId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFIDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RFIDocuments_RFI_RFIId",
                        column: x => x.RFIId,
                        principalTable: "RFI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RFI_POMasterId",
                table: "RFI",
                column: "POMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RFIData_POMasterId",
                table: "RFIData",
                column: "POMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RFIData_RFIId",
                table: "RFIData",
                column: "RFIId");

            migrationBuilder.CreateIndex(
                name: "IX_RFIDocuments_RFIId",
                table: "RFIDocuments",
                column: "RFIId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RFIData");

            migrationBuilder.DropTable(
                name: "RFIDocuments");

            migrationBuilder.DropTable(
                name: "RFI");
        }
    }
}
