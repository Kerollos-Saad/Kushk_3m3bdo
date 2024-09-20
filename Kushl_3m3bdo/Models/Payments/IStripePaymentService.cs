using Kushl_3m3bdo.Models.ViewModels;
using Stripe.Checkout;

namespace Kushl_3m3bdo.Models.Payments
{
	public interface IStripePaymentService
	{
		Task<Session> CreatePlanCheckoutSession(ChargeWalletPlan plan);
	}
}
