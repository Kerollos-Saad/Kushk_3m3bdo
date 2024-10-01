using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.ViewModels;
using System.Linq.Expressions;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
    public interface IApplicationUserRepository
	{
		Task<IEnumerable<UserViewModel>> GetUsers();
		Task<IEnumerable<ApplicationUser>> GetApplicationUsers(Expression<Func<ApplicationUser, bool>>? filter, string[] includeProperties = null);
		Task<ApplicationUser> GetById(string UserId);

		Task<bool> IsInRoleAsyncByRoleName(ApplicationUser user, string roleName);

		Task<IEnumerable<String>> GetUserRoles(ApplicationUser user);

		Task<bool> AddUserToRole(ApplicationUser user, string roleName);
		Task<bool> RemoveUserToRole(ApplicationUser user, string roleName);
	}
}
