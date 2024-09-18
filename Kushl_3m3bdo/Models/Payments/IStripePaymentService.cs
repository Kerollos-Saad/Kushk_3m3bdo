using Kushl_3m3bdo.Models.ViewModels;

namespace Kushl_3m3bdo.Models.Payments
{
	public interface IStripePaymentService
	{
		string CreatePlanCheckoutSession(ChargeWalletPlan plan);
	}
}
