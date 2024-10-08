﻿using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.Consts;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Controllers
{
	[Authorize]
	public class WalletsController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;

		public WalletsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			this._unitOfWork = unitOfWork;
			this._userManager = userManager;
		}

		public async Task<Dictionary<int, string>> PlansDictionary()
		{
			var result = new Dictionary<int, string>();

			var plans = SubscriptionPlan.FetchPlans();
			foreach (var plan in plans)
			{
				result[plan.Id] = plan.Name;
			}

			return result;
		}

		public async Task<WalletViewModel> InitiateWalletViewModel(ApplicationUser user, Wallet targetWallet)
		{
			var walletViewModel = new WalletViewModel
			{
				UserId = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				UserName = user.UserName,
				ProfilePicture = user.ProfilePic,

				WalletId = targetWallet.Id,
				Amount = targetWallet.Amount,
				IsDebts = targetWallet.IsDebts,
				DebtRequest = targetWallet.DebtRequest,

				SubscriptionPlanId = targetWallet.SubscriptionPlanId,
				IsSubscribeToPlan = targetWallet.IsSubscribeToPlan,
				NumberOfSubscriptionPlans = targetWallet.NumberOfSubscriptionPlans,
				PlanSubscriptionStartDate = targetWallet.PlanSubscriptionStartDate,
				WalletCreatedDate = targetWallet.WalletCreatedDate,
				NumberOfPurchases = targetWallet.NumberOfPurchases,
				PriceOfPurchases = targetWallet.PriceOfPurchases,
				Score = targetWallet.Score,
			};
			return walletViewModel;
		}

		public async Task<WalletViewModel> WalletToWalletViewModelConverter(Wallet targetWallet)
		{
			var user = await _userManager.FindByIdAsync(targetWallet.ApplicationUserId);

			var walletViewModel = await InitiateWalletViewModel(user, targetWallet);

			return walletViewModel;
		}

		[HttpGet]
		[HttpPost]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return RedirectToAction("Index", "Users");

			if (user.WalletId != null)
			{
				var targetWallet = await _unitOfWork.Wallets.GetByIdAsync(user.WalletId.Value);

				var WalletVM = await InitiateWalletViewModel(user, targetWallet);

				return View(WalletVM);
			}
			else
			{
				var WalletVM = new WalletViewModel
				{
					UserId = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					FirstName = user.FirstName,
					LastName = user.LastName,
					ProfilePicture = user.ProfilePic
				};

				return View(WalletVM);
			}
		}

		[HttpGet]
		public async Task<IActionResult> CreateWallet()
		{
			var user = await _userManager.Users.Include(u => u.Wallet)
				.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

			if (user.Wallet != null)
			{
				return BadRequest("User already has a wallet.");
			}

			var wallet = new Wallet
			{
				Amount = 0,
				ApplicationUserId = user.Id
			};

			user.Wallet = wallet;

			await _unitOfWork.Wallets.AddAsync(wallet);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Remove(int walletId)
		{
			var user = await _userManager.Users.Include(u => u.Wallet)
				.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

			if (user.Wallet == null)
			{
				return BadRequest("User doesn't have a wallet.");
			}

			var wallet = user.Wallet;

			await _unitOfWork.Wallets.RemoveAsync(wallet);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> RemoveAdministration(int walletId)
		{
			// Why We Use Version For Administration ?
			// Cause Try Two Techniques
			// Why We Didn't Get the Wallet Direct From DB By wallet Id
			//
			// Cause We need a reference for a wallet ==> foreign key constraint violation
			//
			// If i want to get the wallet direct and delete it i should handle the reference of walletId at User before Delete
			// Like get the user and then ==> user.walletId = null
			// Then delete the wallet 
			// Or get the reference at The Next Way ...

			var user = await _userManager.Users.Include(u => u.Wallet)
				.FirstOrDefaultAsync(u => u.WalletId == walletId);

			if (user.Wallet == null)
			{
				return BadRequest("Hi Admin, This User doesn't have a wallet.");
			}

			var wallet = user.Wallet;

			await _unitOfWork.Wallets.RemoveAsync(wallet);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[AutoValidateAntiforgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> ManageWallets()
		{
			ViewData["PlansDict"] = await PlansDictionary();

			var wallets = await _unitOfWork.Wallets.GetAllAsync();

			var walletsViewModel = new List<WalletViewModel>();

			foreach (var wallet in wallets)
			{
				walletsViewModel.Add(await WalletToWalletViewModelConverter(wallet));
			}

			return View(walletsViewModel);
		}

		[HttpGet]
		[AutoValidateAntiforgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> WalletX(int walletId, String? userId)
		{
			if (walletId == 0)
			{
				var user = await _userManager.FindByIdAsync(userId);
				var WalletVM = new WalletViewModel
				{
					UserId = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					FirstName = user.FirstName,
					LastName = user.LastName,
					ProfilePicture = user.ProfilePic
				};

				return View(nameof(Index), WalletVM);
			}

			// User Have Wallet
			var targetWallet = await _unitOfWork.Wallets.GetByIdAsync(walletId);

			var targetWalletViewModel = await WalletToWalletViewModelConverter(targetWallet);

			return View("Index", targetWalletViewModel);

		}
	}
}
