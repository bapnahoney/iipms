using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdatetblSAPUsrPO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorNo",
                table: "SAPUserPOMap",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VendorNo",
                table: "SAPUserPOMap");
        }
    }
}
