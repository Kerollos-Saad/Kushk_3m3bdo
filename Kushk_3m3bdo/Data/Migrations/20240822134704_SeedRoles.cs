using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushk_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.InsertData(
		        table: "Roles",
		        schema: "security",
		        columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
		        values: new Object[] { Guid.NewGuid().ToString(), "User", "User".ToUpper(), Guid.NewGuid().ToString() }
	        );

	        migrationBuilder.InsertData(
		        table: "Roles",
		        schema: "security",
		        columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
		        values: new Object[] { Guid.NewGuid().ToString(), "Admin", "Admin".ToUpper(), Guid.NewGuid().ToString() }
	        );

	        migrationBuilder.InsertData(
		        table: "Roles",
		        schema: "security",
		        columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
		        values: new Object[] { Guid.NewGuid().ToString(), "Manager", "Manager".ToUpper(), Guid.NewGuid().ToString() }
	        );
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("DELETE FROM [security].[Roles] WHERE Name in ('User',Admin','Manager')");
        }
    }
}
