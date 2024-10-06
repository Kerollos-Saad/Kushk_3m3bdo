using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Kushk_3m3bdo.Models.Consts;
using Microsoft.AspNetCore.Authorization;

namespace Kushk_3m3bdo.Controllers
{
	[Authorize]
	public class DebitsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IApplicationUserRepository _userRepository;

		public DebitsController(IUnitOfWork unitOfWork, IApplicationUserRepository userRepository)
		{
			this._unitOfWork = unitOfWork;
			this._userRepository = userRepository;
		}

		private async Task<ApplicationUser> GetCurrentUser()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);

			return applicationUser;
		}

		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> Index()
		{
			var debits =
				await _unitOfWork.Debits.FindAllAsync(includeProperties: new[] { "User", "Wallet" });

			return View(debits);
		}

		[HttpGet]
		public async Task<IActionResult> Add(int amount, int walletId, string userId)
		{
			var wallet = await _unitOfWork.Wallets.FindAsync(w => w.Id == walletId);
			if (wallet != null)
				wallet.DebtRequest = true;

			var newDebit = new Debit()
			{
				Amount = amount,
				RequestDateTime = DateTime.UtcNow,
				UserId = userId,
				AdminId = userId,
				WalletId = walletId,
				Status = DebitStatus.DebitStatusProcessing,
				Wallet = wallet,
			};

			await _unitOfWork.Debits.AddAsync(newDebit);
			await _unitOfWork.SaveAsync();

			TempData["success"] = "Admins Received Your Request";

			if (User.IsInRole(Roles.Role_Manager) ||
			    User.IsInRole(Roles.Role_Admin) ||
			    User.IsInRole(Roles.Role_SubAdmin))
				return RedirectToAction(nameof(Index));
			else
				return RedirectToAction(nameof(MyDebitRequests), new { id = userId });
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> Accept(int id)
		{
			var targetDebit = await _unitOfWork.Debits.FindAsync(d => d.Id == id, new[] { "Wallet" });

			var currentUser = await GetCurrentUser();

			if (currentUser.Id == targetDebit.UserId)
			{
				TempData["error"] = "Nahhh You Can't Accept Your Debit Requests!";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				targetDebit.Status = DebitStatus.DebitStatusApproved;
				targetDebit.AdminId = currentUser.Id;
				targetDebit.ResponseDateTime = DateTime.UtcNow;

				targetDebit.Wallet.Amount += targetDebit.Amount;
				targetDebit.Wallet.IsDebts = (targetDebit.Wallet.Amount < 0);
				targetDebit.Wallet.DebtRequest = false;

				await _unitOfWork.SaveAsync();

				TempData["success"] = "Debit Request Was Accepted";

				return RedirectToAction("WalletX", "Wallets", new { walletId = targetDebit.WalletId });

			}
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> Refuse(int id)
		{
			var targetDebit = await _unitOfWork.Debits.FindAsync(d => d.Id == id, new[] { "Wallet" });

			var currentUser = await GetCurrentUser();

			if (currentUser.Id == targetDebit.UserId)
			{
				TempData["error"] = "Nahhh You Can't Refused Your Debit Requests!";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				targetDebit.Status = DebitStatus.DebitStatusDenied;
				targetDebit.AdminId = currentUser.Id;
				targetDebit.ResponseDateTime = DateTime.UtcNow;

				targetDebit.Wallet.DebtRequest = false;

				await _unitOfWork.SaveAsync();

				TempData["error"] = "Debit Request Was Refused";

				return RedirectToAction("WalletX", "Wallets", new { walletId = targetDebit.WalletId });
			}

		}

		[HttpGet]
		public async Task<IActionResult> MyDebitRequests(string id)
		{
			var debits =
				await _unitOfWork.Debits.FindAllAsync(d => d.UserId == id,
					includeProperties: new[] { "User", "Wallet" });

			return View(nameof(Index), debits);
		}

	}

}
