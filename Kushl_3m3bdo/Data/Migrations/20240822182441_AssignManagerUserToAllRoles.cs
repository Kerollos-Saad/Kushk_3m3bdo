using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushl_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssignManagerUserToAllRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
				"INSERT INTO [security].[UserRoles] (UserId, RoleId) SELECT '27acda15-560f-4082-9f18-5cbfd550f1cd', Id FROM [security].[Roles]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
				"DELETE FROM [security].[UserRoles] WHERE UserId = '27acda15-560f-4082-9f18-5cbfd550f1cd'");
        }
    }
}
