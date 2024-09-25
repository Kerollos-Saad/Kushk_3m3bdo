using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kushl_3m3bdo.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
	public class UsersController : Controller
	{
		private readonly IApplicationUserRepository _userRepository;
		private readonly IIdentityRoleReposssitory _roleRepository;

		public UsersController(IApplicationUserRepository applicationUserRepository, IIdentityRoleReposssitory identityRoleReposssitory)
		{
			this._userRepository = applicationUserRepository;
			this._roleRepository = identityRoleReposssitory;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetUsers();
			return View(users);
		}

		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> ManageRoles(string UserId)
		{
			var user = await _userRepository.GetById(UserId);

			if (user == null)
				return NotFound();

			var roles = await _roleRepository.GetRoles();

			var viewModel = new UserRolesViewModel
			{
				UserId = user.Id,
				UserName = user.UserName,
				Roles = roles.Select(role => new RoleViewModel
				{
					RoleName = role.Name,
					IsSelected = _userRepository.IsInRoleAsyncByRoleName(user, role.Name).Result,
				}).ToList()
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Manager")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
		{

			var user = await _userRepository.GetById(model.UserId);

			if(user == null)
				return NotFound();

			var userRoles = await _userRepository.GetUserRoles(user);

			foreach (var role in model.Roles)
			{
				// Two way to make it async write await or assign to variable

				if (role.IsSelected && !userRoles.Any(r => r == role.RoleName))
					await _userRepository.AddUserToRole(user, role.RoleName);
				
				if (!role.IsSelected && userRoles.Any(r => r == role.RoleName))
				{
					var rm = _userRepository.RemoveUserToRole(user, role.RoleName);
				}
				
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
