using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePOItemForService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "POItems");

            migrationBuilder.AddColumn<float>(
                name: "ServiceQty",
                table: "POItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "ServiceUOM",
                table: "POItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceQty",
                table: "POItems");

            migrationBuilder.DropColumn(
                name: "ServiceUOM",
                table: "POItems");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "POItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
