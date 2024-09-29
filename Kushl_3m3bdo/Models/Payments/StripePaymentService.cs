using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Policy;
using Kushl_3m3bdo.Data.Repository.IRepository;

namespace Kushl_3m3bdo.Models.Payments
{
	public class StripePaymentService : IStripePaymentService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public StripePaymentService( IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		private readonly String _domain = "https://localhost:44308/";
		public async Task<Session> CreatePlanCheckoutSession(ChargeWalletPlan plan)
		{
			var options = new SessionCreateOptions
			{
				SuccessUrl = _domain + $"payments/CheckoutSuccess?planId={plan.Id}&sessionId="+"{CHECKOUT_SESSION_ID}",
				CancelUrl = _domain + $"wallets/index",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};

			var sessionLineItem = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmountDecimal = (long)(plan.Price * 100), // Stripe works in cents
					Currency = "USD",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = plan.Name,
						Description = String.Join(",\n",plan.Options),
						Images = new List<string>() {plan.ImgSrc}
					}
				},
				Quantity = 1
			};

			options.LineItems.Add(sessionLineItem);

			var service = new SessionService();
			Session session = await service.CreateAsync(options);

			return session;
		}

		public async Task<Session> CreatePlanCheckoutSessionAdministration(ChargeWalletPlan plan, int walletId)
		{
			var options = new SessionCreateOptions
			{
				SuccessUrl = _domain + $"payments/CheckoutSuccessAdministration?planId={plan.Id}&walletId={walletId}",
				CancelUrl = _domain + $"wallets/index",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};

			var sessionLineItem = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmountDecimal = (long)(plan.Price * 100), // Stripe works in cents
					Currency = "USD",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = plan.Name,
						Description = String.Join(",\n", plan.Options),
						Images = new List<string>() { plan.ImgSrc }
					}
				},
				Quantity = 1
			};

			options.LineItems.Add(sessionLineItem);

			var service = new SessionService();
			Session session = await service.CreateAsync(options);

			return session;
		}

		public async Task<Session> CreateCheckoutSessionForShoppingCart(
			IEnumerable<ShoppingCart> shoppingCartList, int orderHeaderId)
		{

			var options = new SessionCreateOptions
			{
				SuccessUrl = _domain + $"carts/OrderConfirmation?id={orderHeaderId}",
				CancelUrl = _domain + $"carts/OrderCancelled?id={orderHeaderId}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in shoppingCartList)
			{
				//var baseUrl = "https://faf6-102-43-232-202.ngrok-free.app"; // ngrok

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
				var imageUrl = $"{baseUrl}/carts/GetProductImage?productId={item.Product.Id}";

				//var imageUrl = "https://via.placeholder.com/150";

				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)((item.Product.Price * (decimal)(1-(item.Product.Discount/100))) * 100),
						Currency = "USD",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
							Description = item.Product.Description,
							Images = new List<string>() { imageUrl },
						}
					},
					Quantity = item.Quantity,
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = await service.CreateAsync(options);

			return session;
		}

		
	}
}
