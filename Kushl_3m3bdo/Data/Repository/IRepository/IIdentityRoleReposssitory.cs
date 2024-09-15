using Microsoft.AspNetCore.Identity;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IIdentityRoleReposssitory
	{
		Task<IEnumerable<IdentityRole>> GetRoles();

		Task<IdentityRole> GetRoleById(string  roleId);

		Task<bool> CheckRoleExistByName(string roleName);

		Task<bool> CreateRole(string roleName);

		Task<bool> RemoveRole(string roleId);
	}
}
