using System.ComponentModel.DataAnnotations.Schema;

namespace Kushk_3m3bdo.Models
{
	public class Wallet
	{
		public int Id { get; set; }
		public decimal Amount { get; set; } = 0;
		public bool IsDebts { get; set; } = false;
		public bool DebtRequest { get; set; } = false;

		public int SubscriptionPlanId { get; set; } = 0;
		public bool IsSubscribeToPlan { get; set; } = false;
		public int NumberOfSubscriptionPlans { get; set; } = 0;
		public DateTime PlanSubscriptionStartDate { get; set; }
		public DateTime WalletCreatedDate { get; set; } = DateTime.UtcNow;
		public int NumberOfPurchases { get; set; } = 0;
		public decimal PriceOfPurchases { get; set; } = 0;
		public double Score { get; set; } = 100.0;


		[ForeignKey("User")]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser User { get; set; }

	}
}
