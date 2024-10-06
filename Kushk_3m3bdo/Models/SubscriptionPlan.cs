using Kushk_3m3bdo.Models.ViewModels;

namespace Kushk_3m3bdo.Models
{
	public class SubscriptionPlan
	{
		private static List<ChargeWalletPlan> paymentPlans = new List<ChargeWalletPlan>
		{
			new ChargeWalletPlan { Id = 1, Name = "Starter", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 100M, AdditionalCreditPercentage = 10.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Economic Plan"}},
			new ChargeWalletPlan { Id = 2, Name = "Pro 3bdo", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 200M, AdditionalCreditPercentage = 20.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Get Notifications For Offers", "Free Support"}},
			new ChargeWalletPlan { Id = 3, Name = "Company", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 1000M, AdditionalCreditPercentage = 25.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Get Notifications For Offers", "Free Support", "Free Shipping"}}
		};


		public static List<ChargeWalletPlan> FetchPlans()
		{
			return paymentPlans;
		}

		public static void AddPlan(ChargeWalletPlan paymentPlan)
		{
			paymentPlans.Add(paymentPlan);
		}

		public static void RemovePlan(ChargeWalletPlan paymentPlan)
		{
			paymentPlans.Remove(paymentPlan);
		}

	}
}
