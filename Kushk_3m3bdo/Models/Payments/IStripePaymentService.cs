using Kushk_3m3bdo.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Stripe.Checkout;

namespace Kushk_3m3bdo.Models.Payments
{
	public interface IStripePaymentService
	{
		Task<Session> CreatePlanCheckoutSession(ChargeWalletPlan plan);
		Task<Session> CreatePlanCheckoutSessionAdministration(ChargeWalletPlan plan, int walletId);

		Task<Session> CreateCheckoutSessionForShoppingCart(IEnumerable<ShoppingCart> shoppingCartList, int orderHeaderId);
	}
}
