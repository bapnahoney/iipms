using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HIPMS.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "ICData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InspectionById",
                table: "ICData",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferedOn",
                table: "ICData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "ICData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedOn",
                table: "ICData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rejecter",
                table: "ICData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "ICData");

            migrationBuilder.DropColumn(
                name: "InspectionById",
                table: "ICData");

            migrationBuilder.DropColumn(
                name: "ReferedOn",
                table: "ICData");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "ICData");

            migrationBuilder.DropColumn(
                name: "RejectedOn",
                table: "ICData");

            migrationBuilder.DropColumn(
                name: "Rejecter",
                table: "ICData");
        }
    }
}
