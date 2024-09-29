using System.Linq.Expressions;
using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Kushl_3m3bdo.Data.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
	{

		private readonly UserManager<ApplicationUser> _userManager;
		public ApplicationUserRepository(UserManager<ApplicationUser> userManager)
		{
			this._userManager = userManager;
		}

		public async Task<IEnumerable<UserViewModel>> GetUsers()
		{
			var users = _userManager.Users.Select(
				user => new UserViewModel
				{
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					UserName = user.UserName,
					Email = user.Email,
					ProfilePicture = user.ProfilePic,
					Roles = _userManager.GetRolesAsync(user).Result
				}).ToList();

			return users;
		}

		public async Task<IEnumerable<ApplicationUser>> GetApplicationUsers(Expression<Func<ApplicationUser, bool>>? filter, string[] includeProperties = null)
		{
			IQueryable<ApplicationUser> query = _userManager.Users;

			includeProperties ??= Array.Empty<string>();
			foreach (var property in includeProperties)
				query = query.Include(property);

			if(filter != null)
				query = query.Where(filter);

			return await query.ToListAsync();
		}


		public async Task<ApplicationUser> GetById(string UserId)
		{
			var user = await _userManager.FindByIdAsync(UserId);
			return user;
		}

		public async Task<bool> IsInRoleAsyncByRoleName(ApplicationUser user, string roleName)
		{
			return await _userManager.IsInRoleAsync(user, roleName);
		}

		public async Task<IEnumerable<string>> GetUserRoles(ApplicationUser user)
		{
			return await _userManager.GetRolesAsync(user);
		}

		public async Task<bool> AddUserToRole(ApplicationUser user, string roleName)
		{
			var newRole = await _userManager.AddToRoleAsync(user, roleName);
			return newRole.Succeeded;
		}

		public async Task<bool> RemoveUserToRole(ApplicationUser user, string roleName)
		{
			var oldRole = await _userManager.RemoveFromRoleAsync(user, roleName);
			return oldRole.Succeeded;
		}
	}
}
