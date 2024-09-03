using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class updateTblDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RFIDataId",
                table: "RFIDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ICDataId",
                table: "ICDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RFIDocuments_RFIDataId",
                table: "RFIDocuments",
                column: "RFIDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ICDocuments_ICDataId",
                table: "ICDocuments",
                column: "ICDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_ICDocuments_ICData_ICDataId",
                table: "ICDocuments",
                column: "ICDataId",
                principalTable: "ICData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RFIDocuments_RFIData_RFIDataId",
                table: "RFIDocuments",
                column: "RFIDataId",
                principalTable: "RFIData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ICDocuments_ICData_ICDataId",
                table: "ICDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_RFIDocuments_RFIData_RFIDataId",
                table: "RFIDocuments");

            migrationBuilder.DropIndex(
                name: "IX_RFIDocuments_RFIDataId",
                table: "RFIDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ICDocuments_ICDataId",
                table: "ICDocuments");

            migrationBuilder.DropColumn(
                name: "RFIDataId",
                table: "RFIDocuments");

            migrationBuilder.DropColumn(
                name: "ICDataId",
                table: "ICDocuments");
        }
    }
}
