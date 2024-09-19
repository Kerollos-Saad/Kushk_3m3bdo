using System.ComponentModel.DataAnnotations.Schema;

namespace Kushl_3m3bdo.Models
{
	public class Wallet
	{
		public int Id { get; set; }
		public decimal Amount { get; set; } = 0;
		public bool isDebts { get; set; } = false;

		public int SubscriptionPlanId { get; set; } = 0;
		public bool IsSubscripeToPlan { get; set; } = false;
		public DateTime PlanSubscriptionStrated { get; set; } = DateTime.UtcNow;
		public double Score { get; set; } = 100.0;

		[ForeignKey("User")]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser User { get; set; }

	}
}
