namespace Kushk_3m3bdo.Models.ViewModels
{
	public class ChargeWalletPlan
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public string ImgSrc { get; set; }

		public decimal Price{ get; set; }
		public double AdditionalCreditPercentage { get; set; }

		public DateTime NextSubscription { get; set; } = DateTime.UtcNow;

		public IEnumerable<String> Options = new List<string>();
	}
}
