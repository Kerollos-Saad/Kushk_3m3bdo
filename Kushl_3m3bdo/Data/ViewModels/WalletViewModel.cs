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
		public double Amount { get; set; } = 0;
		public bool isDebts { get; set; } = false;
	}
}
