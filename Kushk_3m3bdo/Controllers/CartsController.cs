using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Kushk_3m3bdo.Models.Consts;
using Kushk_3m3bdo.Models.Payments;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Stripe.Checkout;
using Microsoft.AspNetCore.Authorization;

namespace Kushk_3m3bdo.Controllers
{
	[Authorize]
	public class CartsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IApplicationUserRepository _userRepository;
		private readonly IStripePaymentService _stripePaymentService;

		[BindProperty] public ShoppingCartsViewModel ShoppingCartsViewModel { get; set; }

		public CartsController(IUnitOfWork unitOfWork, IApplicationUserRepository userRepository,
			IStripePaymentService stripePaymentService)
		{
			this._unitOfWork = unitOfWork;
			this._userRepository = userRepository;
			this._stripePaymentService = stripePaymentService;
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
				ShoppingCartList =
					await _unitOfWork.ShoppingCarts.FindAllAsync(c => c.ApplicationUserId == currentUser.Id, null, null,
						new[] { "Product" }),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartsViewModel.ShoppingCartList)
			{
				cart.Price = GetPriceAfterDiscount(cart);
				ShoppingCartsViewModel.OrderHeader.OrderTotal += (decimal)(cart.Price * cart.Quantity);
			}

			return View(ShoppingCartsViewModel);
		}

		public async Task<IActionResult> Plus(int cartId)
		{
			ApplicationUser currentUser = await GetCurrentUser();

			var cart = await _unitOfWork.ShoppingCarts.FindAsync(c =>
				c.Id == cartId &&
				c.ApplicationUserId == currentUser.Id, new[] { "Product" });

			if (cart.Quantity + 1 <= cart.Product.Stock)
			{
				//cart.Product.Stock -= 1;			===>   Product Stock Will Change At Check Out
				cart.Quantity += 1;

				//await _unitOfWork.ShoppingCarts.Update(cart); // Without needn't for explicit control
				await _unitOfWork.SaveAsync();
			}
			else
			{
				TempData["StockBecomeEmpty"] = 1;
			}

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Minus(int cartId)
		{
			ApplicationUser currentUser = await GetCurrentUser();

			var cart = await _unitOfWork.ShoppingCarts.FindAsync(c =>
				c.Id == cartId &&
				c.ApplicationUserId == currentUser.Id);

			if (cart.Quantity <= 1)
			{
				return RedirectToAction(nameof(Remove), new { cartId = cartId });
			}

			cart.Quantity -= 1;
			//await _unitOfWork.ShoppingCarts.Update(cart); // Without needn't for explicit control
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Remove(int cartId)
		{
			ApplicationUser currentUser = await GetCurrentUser();

			var cart = await _unitOfWork.ShoppingCarts.FindAsync(c =>
				c.Id == cartId &&
				c.ApplicationUserId == currentUser.Id);

			await _unitOfWork.ShoppingCarts.RemoveAsync(cart);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> PickUser()
		{
			var users = await _userRepository.GetApplicationUsers(null, new[] { "Wallet" });
			return View(users);
		}

		[HttpPost]
		[ActionName("PickUser")]
		public async Task<IActionResult> ContinueToCheckout(string userId)
		{
			return RedirectToAction(nameof(Summary), new { userId = userId });
		}

		[HttpGet]
		public async Task<IActionResult> Summary(string? userId)
		{
			//var cartOwnership = userId == null ? "Admin Cart" : $"User {userId} cart";
			//return Json("Summary Get " + cartOwnership);


			var orderUser = userId != null ? await _userRepository.GetById(userId) : await GetCurrentUser();
			var currentUser = await GetCurrentUser();

			ShoppingCartsViewModel = new()
			{
				ShoppingCartList = await _unitOfWork.ShoppingCarts.FindAllAsync(
					c => c.ApplicationUserId == currentUser.Id,
					null, null, new[] { "Product" }),
				OrderHeader = new()
			};

			if (userId != null)
			{
				ShoppingCartsViewModel.OrderHeader.SalesId = currentUser.Id;
			}


			ShoppingCartsViewModel.OrderHeader.ApplicationUserId = orderUser.Id;

			ShoppingCartsViewModel.OrderHeader.Name = orderUser.UserName;
			ShoppingCartsViewModel.OrderHeader.PhoneNumber = orderUser.PhoneNumber;
			ShoppingCartsViewModel.OrderHeader.StreetAddress = orderUser.StreetAddress;
			ShoppingCartsViewModel.OrderHeader.City = orderUser.City;
			ShoppingCartsViewModel.OrderHeader.State = orderUser.State;
			ShoppingCartsViewModel.OrderHeader.ZipCode = orderUser.ZipCode;

			foreach (var cart in ShoppingCartsViewModel.ShoppingCartList)
			{
				cart.Price = GetPriceAfterDiscount(cart);
				ShoppingCartsViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
			}

			TempData["UserId"] = orderUser.Id;
			TempData["SalesId"] = currentUser.Id;

			return View(ShoppingCartsViewModel);
		}

		[HttpPost]
		[ActionName("Summary")]
		public async Task<IActionResult> SummaryPost(bool? isDelivery)
		{
			TempData["IsDelivery"] = isDelivery;

			// Append User and Sales To Shopping Cart View Model
			ShoppingCartsViewModel.OrderHeader.ApplicationUserId = TempData["UserId"].ToString();
			ShoppingCartsViewModel.OrderHeader.SalesId = TempData["SalesId"].ToString();

			TempData["ShoppingCartsViewModel"] = JsonConvert.SerializeObject(ShoppingCartsViewModel);

			//return Json($"{ShoppingCartsViewModel.OrderHeader.ApplicationUserId} + {ShoppingCartsViewModel.OrderHeader.SalesId}");
			return RedirectToAction("PaymentType");
		}

		[HttpGet]
		[HttpPost]
		[ActionName("SummaryPostAfterPayment")]
		public async Task<IActionResult> SummaryPostAfterPayment()
		{
			// Retrieve values from TempData

			string? shoppingCartModelJson = TempData.Peek("ShoppingCartsViewModel") as string;
			ShoppingCartsViewModel = JsonConvert.DeserializeObject<ShoppingCartsViewModel>(shoppingCartModelJson);

			bool? isDelivery = TempData.Peek("IsDelivery") as bool?;
			bool? isCredit = TempData.Peek("IsCredit") as bool?;


			// Continue processing based on the payment type result
			var currentUser = await GetCurrentUser();

			ShoppingCartsViewModel.OrderHeader.OrderType = isDelivery == true ? OrderType.Delivery : OrderType.Pickup;
			ShoppingCartsViewModel.OrderHeader.PaymentType =
				isCredit == true ? PaymentTypes.Credit : PaymentTypes.Wallet;

			ShoppingCartsViewModel.ShoppingCartList =
				await _unitOfWork.ShoppingCarts.FindAllAsync(c => c.ApplicationUserId == currentUser.Id,
					null, null, new[] { "Product" });

			ShoppingCartsViewModel.OrderHeader.OrderDate = DateTime.UtcNow;
			ShoppingCartsViewModel.OrderHeader.ApplicationUserId =
				ShoppingCartsViewModel.OrderHeader.ApplicationUserId;
			//ShoppingCartsViewModel.OrderHeader.ApplicationUserId = currentUser.Id;

			ShoppingCartsViewModel.OrderHeader.OrderStatus = OrderStatus.StatusPending;
			ShoppingCartsViewModel.OrderHeader.PaymentStatus = PaymentStatus.PaymentStatusPending;

			// Didn't need This Loop Was Repeated
			foreach (var cart in ShoppingCartsViewModel.ShoppingCartList)
			{
				cart.Price = GetPriceAfterDiscount(cart);
				ShoppingCartsViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
			}

			await _unitOfWork.OrderHeaders.AddAsync(ShoppingCartsViewModel.OrderHeader);
			await _unitOfWork.SaveAsync();

			foreach (var cart in ShoppingCartsViewModel.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					Quantity = cart.Quantity,
					Price = cart.Product.Price, // Original Price
					Discount = cart.Product.Discount, // Discount Price
					OrderHeaderId = ShoppingCartsViewModel.OrderHeader.Id
				};
				await _unitOfWork.OrderDetails.AddAsync(orderDetail);
				await _unitOfWork.SaveAsync();
			}

			//return Json(ShoppingCartsViewModel.ShoppingCartList);

			if (isCredit == true)
			{
				// Payment Credit Via Stripe
				var session =
					await _stripePaymentService.CreateCheckoutSessionForShoppingCart(
						ShoppingCartsViewModel.ShoppingCartList, ShoppingCartsViewModel.OrderHeader.Id);

				await _unitOfWork.OrderHeaders.UpdateStripePaymentId(ShoppingCartsViewModel.OrderHeader.Id, session.Id,
					session.PaymentIntentId);
				await _unitOfWork.SaveAsync();

				return Redirect(session.Url);
			}
			else
			{
				// Payment From User Wallet

				var walletUser = await _unitOfWork.Wallets.FindAsync(w =>
					w.ApplicationUserId == ShoppingCartsViewModel.OrderHeader.ApplicationUserId);
				if (walletUser != null)
				{
					if (walletUser.Amount >= ShoppingCartsViewModel.OrderHeader.OrderTotal)
					{
						// Enough Wallet Amount For This Purchases
						walletUser.Amount -= ShoppingCartsViewModel.OrderHeader.OrderTotal;
						walletUser.NumberOfPurchases += ShoppingCartsViewModel.ShoppingCartList.Count();

						await _unitOfWork.OrderHeaders.UpdateStatus(ShoppingCartsViewModel.OrderHeader.Id,
							OrderStatus.StatusApproved, PaymentStatus.PaymentStatusApproved);
						await _unitOfWork.SaveAsync();

						IEnumerable<ShoppingCart> shoppingCarts =
							await _unitOfWork.ShoppingCarts.FindAllAsync(
								c => c.ApplicationUserId == ShoppingCartsViewModel.OrderHeader.ApplicationUserId,
								null, null);

						await _unitOfWork.ShoppingCarts.RemoveRangeAsync(shoppingCarts);
						await _unitOfWork.SaveAsync();

						return RedirectToAction(nameof(OrderConfirmation),
							new { id = ShoppingCartsViewModel.OrderHeader.Id });
					}
					else
					{
						// Wallet Amount Less Than Order Total
						TempData["WalletError"] = "Target Wallet Doesn't Have Enough Money For This Order!";
						return RedirectToAction(nameof(OrderCancelled),
							new { id = ShoppingCartsViewModel.OrderHeader.Id });
					}
				}
				else
				{
					TempData["WalletError"] = "User Should have a Wallet To Pay!";
					return RedirectToAction(nameof(OrderCancelled),
						new { id = ShoppingCartsViewModel.OrderHeader.Id });
				}
			}
		}

		[HttpGet]
		public async Task<IActionResult> PaymentType()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> PaymentType(bool? isCredit)
		{
			TempData["IsCredit"] = isCredit;

			return RedirectToAction("SummaryPostAfterPayment");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetProductImage(int productId)
		{
			var product = await _unitOfWork.Products.FindAsync(p => p.Id == productId);
			if (product == null || product.ProductImg == null)
			{
				return NotFound();
			}

			// Assuming the product image is stored as a byte array
			return File(product.ProductImg, "image/jpeg"); // Change the content type accordingly
		}

		public async Task<IActionResult> OrderConfirmation(int id)
		{
			OrderHeader orderHeader =
				await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == id, new[] { "ApplicationUser" });
			if (orderHeader.PaymentStatus != PaymentStatus.PaymentStatusDelayedPayment)
			{
				//this is an order by customer

				// Stripe Payment
				if (!String.IsNullOrEmpty(orderHeader.SessionId))
				{
					var service = new SessionService();
					Session session = await service.GetAsync(orderHeader.SessionId);

					if (session.PaymentStatus.ToLower() == "paid")
					{
						await _unitOfWork.OrderHeaders.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					}
				}

				// Stripe Payment | Wallet Payment

				await _unitOfWork.OrderHeaders.UpdateStatus(id, OrderStatus.StatusApproved,
					PaymentStatus.PaymentStatusApproved);
				await _unitOfWork.SaveAsync();
			}

			IEnumerable<ShoppingCart> shoppingCarts =
				await _unitOfWork.ShoppingCarts.FindAllAsync(c => c.ApplicationUserId == orderHeader.SalesId,
					null, null, new []{"Product"});

			await _unitOfWork.ShoppingCarts.RemoveRangeAsync(shoppingCarts);
			await _unitOfWork.SaveAsync();

			foreach (var cart in shoppingCarts)
			{
				cart.Product.Stock -= cart.Quantity;
			}

			await _unitOfWork.SaveAsync();

			return View(id);
		}

		public async Task<IActionResult> OrderCancelled(int id, bool? walletProblem)
		{
			OrderHeader orderHeader =
				await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == id, new[] { "ApplicationUser" });

			await _unitOfWork.OrderHeaders.UpdateStatus(id, OrderStatus.StatusCancelled, PaymentStatus.PaymentStatusCancelled);
			await _unitOfWork.SaveAsync();

			if (walletProblem == false)
				return RedirectToAction(nameof(Index));
			else
				return RedirectToAction("WalletX", "Wallets",
					new { walletId = orderHeader.ApplicationUser.WalletId, userId = orderHeader.ApplicationUserId });
		}

		private decimal GetPriceAfterDiscount(ShoppingCart cart)
		{
			return cart.Product.Price * (decimal)(1 - (cart.Product.Discount / 100));
		}
	}
}
