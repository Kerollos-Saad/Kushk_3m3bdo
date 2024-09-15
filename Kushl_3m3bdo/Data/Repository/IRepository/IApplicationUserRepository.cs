using Kushl_3m3bdo.Data.ViewModels;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IApplicationUserRepository
	{
		Task<IEnumerable<UserViewModel>> GetUsers();

		Task<ApplicationUser> GetById(string UserId);

		Task<bool> IsInRoleAsyncByRoleName(ApplicationUser user, string roleName);

		Task<IEnumerable<String>> GetUserRoles(ApplicationUser user);

		Task<bool> AddUserToRole(ApplicationUser user, string roleName);
		Task<bool> RemoveUserToRole(ApplicationUser user, string roleName);
	}
}
