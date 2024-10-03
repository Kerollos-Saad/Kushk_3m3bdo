using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models.Consts;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Controllers
{
    [Authorize(Roles = Roles.Role_Manager)]
	public class ManagerRolesController : Controller
	{
		IApplicationUserRepository _userRepository;
		IIdentityRoleReposssitory _roleRepository;

		//private readonly RoleManager<IdentityRole> _roleManager;

		public ManagerRolesController(
			//RoleManager<IdentityRole> roleManager,
			IApplicationUserRepository applicationUserRepository,
			IIdentityRoleReposssitory identityRoleRepository)
		{
			//this._roleManager = roleManager;
			this._userRepository = applicationUserRepository;
			this._roleRepository = identityRoleRepository;
		}

		public async Task<IActionResult> Index()
		{
			//var roles = await _roleManager.Roles.ToListAsync();
			var role = _roleRepository.GetRoles();
			return View(role.Result);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(RoleFormViewModel roleFormVM)
		{
			if (!ModelState.IsValid)
				return View("Index", _roleRepository.GetRoles().Result);

			//var roleIsExits = await _roleManager.RoleExistsAsync(roleFormVM.Name);
			var roleIsExits = _roleRepository.CheckRoleExistByName(roleFormVM.Name).Result;

			if (roleIsExits)
			{
				ModelState.AddModelError("Name", "Role is Exist!");
				return View("Index", _roleRepository.GetRoles().Result);
			}

			//await _roleManager.CreateAsync(new IdentityRole(roleFormVM.Name.Trim()));
			var isSucceeded = _roleRepository.CreateRole(roleFormVM.Name.Trim()).Result;

			if (isSucceeded)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ModelState.AddModelError("Name", "Failed To Create!");
				return View("Index", _roleRepository.GetRoles().Result);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Remove(String roleId)
		{
			if (!ModelState.IsValid)
				return View("Index", _roleRepository.GetRoles().Result);

			var isSucceeded = _roleRepository.RemoveRole(roleId).Result;

			if (isSucceeded)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ModelState.AddModelError("Name", "Failed To Delete!");
				return View("Index", _roleRepository.GetRoles().Result);
			}

		}
	}
}
