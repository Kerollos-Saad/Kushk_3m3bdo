using Kushl_3m3bdo.Models.ViewModels;
using Stripe.Checkout;

namespace Kushl_3m3bdo.Models.Payments
{
	public class StripePaymentService : IStripePaymentService
	{
		private readonly String _domain = "https://localhost:44308/";
		public async Task<Session> CreatePlanCheckoutSession(ChargeWalletPlan plan)
		{
			//var domain = "https://localhost:44308/";
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
			//var domain = "https://localhost:44308/";
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
	}
}
