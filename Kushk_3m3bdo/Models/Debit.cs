using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kushk_3m3bdo.Models
{
	public class Debit
	{
		public int Id { get; set; }

		public decimal Amount { get; set; }

		public DateTime RequestDateTime { get; set; } = DateTime.UtcNow;
		public DateTime ResponseDateTime { get; set; }

		public bool Accepted { get; set; } = false;

		public string UserId { get; set; }
		public string AdminId { get; set; }
		public int WalletId { get; set; }

		[ForeignKey("UserId")]
		[ValidateNever]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("AdminId")]
		[ValidateNever]
		public virtual ApplicationUser AdminUser { get; set; }

		[ForeignKey("WalletId")]
		[ValidateNever]
		public virtual Wallet Wallet { get; set; }

	}
}
