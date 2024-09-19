namespace Kushl_3m3bdo.Data.ViewModels
{
	public class WalletViewModel
	{
		// User Data
		public string UserId { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }

		public byte[] ProfilePicture { get; set; }

		// Wallet Data

		public int WalletId { get; set; }
		public decimal Amount { get; set; } = 0;
		public bool isDebts { get; set; } = false;

		public int SubscriptionPlanId { get; set; } = 0;
		public bool IsSubscripeToPlan { get; set; } = false;
		public DateTime PlanSubscriptionStrated { get; set; } = DateTime.UtcNow;
		public double Score { get; set; } = 100.0;
	}
}
