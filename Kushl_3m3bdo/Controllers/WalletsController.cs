using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Data.ViewModels;
using Kushl_3m3bdo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Controllers
{
	public class WalletsController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;

		public WalletsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
		{
			this._unitOfWork = unitOfWork;
			this._userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user.WalletId != null)
			{
				var targetWallet = await _unitOfWork.Wallets.GetByIdAsync(user.WalletId.Value);

				var WalletVM = new WalletViewModel
				{
					UserId = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					FirstName = user.FirstName,
					LastName = user.LastName,
					ProfilePicture = user.ProfilePic,

					WalletId = targetWallet.Id,
					Amount = targetWallet.Amount,
					isDebts = targetWallet.IsDebts
				};
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
		public async Task<IActionResult> Remove(int WalletId)
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
	}
}
