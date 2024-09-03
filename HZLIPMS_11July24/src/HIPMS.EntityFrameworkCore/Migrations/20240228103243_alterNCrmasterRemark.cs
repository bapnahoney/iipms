using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class alterNCrmasterRemark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserRemark",
                table: "NCRMaster",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRemark",
                table: "NCRMaster");
        }
    }
}
