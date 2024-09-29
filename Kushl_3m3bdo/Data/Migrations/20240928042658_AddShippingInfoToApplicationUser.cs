using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushl_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingInfoToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                schema: "Security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                schema: "Security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                schema: "Security",
                table: "Users");
        }
    }
}
