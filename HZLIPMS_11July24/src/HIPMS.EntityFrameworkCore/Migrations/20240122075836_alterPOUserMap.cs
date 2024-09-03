using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class alterPOUserMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SAPUserPOMap_UserId",
                table: "SAPUserPOMap");

            migrationBuilder.RenameColumn(
                name: "PO",
                table: "SAPUserPOMap",
                newName: "PONo");

            migrationBuilder.AddColumn<bool>(
                name: "IsVendor",
                table: "SAPUserPOMap",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "POId",
                table: "SAPUserPOMap",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SAPUserPOMap_POId",
                table: "SAPUserPOMap",
                column: "POId");

            migrationBuilder.CreateIndex(
                name: "IX_SAPUserPOMap_UserId_POId",
                table: "SAPUserPOMap",
                columns: new[] { "UserId", "POId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SAPUserPOMap_POMaster_POId",
                table: "SAPUserPOMap",
                column: "POId",
                principalTable: "POMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SAPUserPOMap_POMaster_POId",
                table: "SAPUserPOMap");

            migrationBuilder.DropIndex(
                name: "IX_SAPUserPOMap_POId",
                table: "SAPUserPOMap");

            migrationBuilder.DropIndex(
                name: "IX_SAPUserPOMap_UserId_POId",
                table: "SAPUserPOMap");

            migrationBuilder.DropColumn(
                name: "IsVendor",
                table: "SAPUserPOMap");

            migrationBuilder.DropColumn(
                name: "POId",
                table: "SAPUserPOMap");

            migrationBuilder.RenameColumn(
                name: "PONo",
                table: "SAPUserPOMap",
                newName: "PO");

            migrationBuilder.CreateIndex(
                name: "IX_SAPUserPOMap_UserId",
                table: "SAPUserPOMap",
                column: "UserId");
        }
    }
}
