using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class AlterOECdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByOEC",
                table: "RFI",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OECActionRemark",
                table: "RFI",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByOEC",
                table: "InspectionClearance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OECActionRemark",
                table: "InspectionClearance",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedByOEC",
                table: "RFI");

            migrationBuilder.DropColumn(
                name: "OECActionRemark",
                table: "RFI");

            migrationBuilder.DropColumn(
                name: "IsApprovedByOEC",
                table: "InspectionClearance");

            migrationBuilder.DropColumn(
                name: "OECActionRemark",
                table: "InspectionClearance");
        }
    }
}
