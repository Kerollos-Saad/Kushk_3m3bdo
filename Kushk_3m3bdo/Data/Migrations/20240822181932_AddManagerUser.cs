using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kushk_3m3bdo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        "INSERT INTO [security].[Users] ([Id], [ProfilePic], [WalletId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [LastName]) VALUES (N'27acda15-560f-4082-9f18-5cbfd550f1cd', NULL, NULL, N'3m_3bdo', N'3M_3BDO', N'manager@manager.com', N'MANAGER@MANAGER.COM', 0, N'AQAAAAIAAYagAAAAENq3MTRPhlsuxnyLzn1Nwoi+qGCL6L7NO+9FjgUQF9ouMqEsvVPMPSi3mJXkCtR5Kw==', N'2JREUFDZX32S6ZPZJEQUZGDN32ER5HO7', N'af1ea8e9-4011-4c41-8788-66214fcd710c', NULL, 0, 0, NULL, 1, 0, N'3m', N'3bdo')\r\n");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        "DELETE FROM [security].[Users] WHERE Id = '27acda15-560f-4082-9f18-5cbfd550f1cd'");
        }
    }
}
