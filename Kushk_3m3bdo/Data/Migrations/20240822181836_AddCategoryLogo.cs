using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushk_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Categories",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Categories");
        }
    }
}
