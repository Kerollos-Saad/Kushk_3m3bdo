using Kushk_3m3bdo.Data.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Data.Repository
{
	public class IdentityRoleRepository : IIdentityRoleReposssitory
	{

		private readonly RoleManager<IdentityRole> _roleManager;
		public IdentityRoleRepository(RoleManager<IdentityRole> roleManager)
		{
			this._roleManager = roleManager;
		}

		public async Task<IEnumerable<IdentityRole>> GetRoles()
		{
			var roles = await _roleManager.Roles.ToListAsync();
			return roles;
		}

		public async Task<IdentityRole> GetRoleById(string roleId)
		{
			var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
			return role;
		}

		public async Task<bool> CheckRoleExistByName(string roleName)
		{
			return await _roleManager.RoleExistsAsync(roleName);
		}

		public async Task<bool> CreateRole(string roleName)
		{
			var Created = await _roleManager.CreateAsync(new IdentityRole(roleName));
			return Created.Succeeded;
		}

		public async Task<bool> RemoveRole(string roleId)
		{
			var role = await GetRoleById(roleId);

            if (role == null)
                return false;

			var Deleted = await _roleManager.DeleteAsync(role);
			return Deleted.Succeeded;
		}
	}
}
