using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kushl_3m3bdo.Controllers
{
	public class CartsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IApplicationUserRepository _userRepository;

		[BindProperty]
		public ShoppingCartsViewModel ShoppingCartsViewModel { get; set; }

		public CartsController(IUnitOfWork unitOfWork, IApplicationUserRepository userRepository)
		{
			this._unitOfWork = unitOfWork;
			this._userRepository = userRepository;
		}

		public async Task<ApplicationUser> GetCurrentUser()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);

			return applicationUser;
		}

		public async Task<IActionResult> Index()
		{
			ApplicationUser currentUser = await GetCurrentUser();

			ShoppingCartsViewModel = new()
			{
				ShoppingCartList = await _unitOfWork.ShoppingCarts.FindAllAsync(c => c.ApplicationUserId == currentUser.Id, null, null, new[] { "Product" }),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartsViewModel.ShoppingCartList)
			{
				cart.Price = GetPriceAfterDiscount(cart);
				ShoppingCartsViewModel.OrderHeader.OrderTotal += (decimal)(cart.Price * cart.Quantity);
			}

			return View(ShoppingCartsViewModel);
		}


		private decimal GetPriceAfterDiscount(ShoppingCart cart)
		{
			return cart.Product.Price * (decimal)(1 - (cart.Product.Discount / 100));
		}
	}
}
