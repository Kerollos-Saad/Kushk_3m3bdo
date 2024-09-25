namespace Kushl_3m3bdo.Models.ViewModels
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
    }
}
