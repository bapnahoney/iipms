using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class add_POenv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "POEnvironment",
                table: "POMaster",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "POEnvironment",
                table: "POMaster");
        }
    }
}
