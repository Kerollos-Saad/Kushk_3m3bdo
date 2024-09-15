using System.ComponentModel.DataAnnotations.Schema;

namespace Kushl_3m3bdo.Models
{
	public class Wallet
	{
		public int Id { get; set; }
		public double Amount { get; set; } = 0;
		public bool isDebts { get; set; } = false;

		[ForeignKey("User")]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser User { get; set; }

	}
}
