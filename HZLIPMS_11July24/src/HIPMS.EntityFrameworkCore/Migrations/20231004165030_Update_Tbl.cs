using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class Update_Tbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "InspectionClearance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerPlantAddress",
                table: "InspectionClearance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialClass",
                table: "ICData",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "InspectionClearance");

            migrationBuilder.DropColumn(
                name: "ManufacturerPlantAddress",
                table: "InspectionClearance");

            migrationBuilder.DropColumn(
                name: "MaterialClass",
                table: "ICData");
        }
    }
}
