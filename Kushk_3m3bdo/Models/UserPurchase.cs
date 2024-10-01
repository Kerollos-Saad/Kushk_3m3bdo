using System.ComponentModel.DataAnnotations.Schema;

namespace Kushk_3m3bdo.Models
{
	public class UserPurchase
	{
		public int Id { get; set; }

		public int Quantity { get; set; } = 1;

		public DateTime PurchaseTime { get; set; } = DateTime.UtcNow;

		[ForeignKey("User")]
		public string ApplicationUserId { get; set; }

		[ForeignKey("Product")]
		public int ProductId { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual Product Product { get; set; }


	}
}
