using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Kushk_3m3bdo.Controllers
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

		public async Task<IActionResult> IndexWithoutSort()
		{
			var users = await _userRepository.GetUsers();
			return View(users);
		}

		public async Task<IActionResult> Index(string sortOrder)
		{
			var users = await _userRepository.GetUsersWithRolesAndWallet();

			foreach (var user in users)
			{
				if (user.userWallet == null)
				{
					user.userWallet = new Wallet { Amount = 0 };
				}
			}

			ViewData["UserNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewData["WalletAmountSortParm"] = sortOrder == "Amount" ? "amount_desc" : "Amount";
			ViewData["WalletScoreSortParm"] = sortOrder == "Score" ? "score_desc" : "Score";

			switch (sortOrder)
			{
				case "name_desc":
					users = users.OrderByDescending(s => s.UserName);
					break;
				case "Amount":
					users = users.OrderBy(s => s.userWallet.Amount);
					break;
				case "amount_desc":
					users = users.OrderByDescending(s => s.userWallet.Amount);
					break;
				case "Score":
					users = users.OrderBy(s => s.userWallet.Score);
					break;
				case "score_desc":
					users = users.OrderByDescending(s => s.userWallet.Score);
					break;
				default:
					users = users.OrderBy(s => s.UserName);
					break;
			}
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
					await _userRepository.RemoveUserToRole(user, role.RoleName);
				}
				
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
