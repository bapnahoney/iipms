using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRFI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ServiceQty",
                table: "RFIData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ServiceUOM",
                table: "RFIData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceQty",
                table: "RFIData");

            migrationBuilder.DropColumn(
                name: "ServiceUOM",
                table: "RFIData");
        }
    }
}
