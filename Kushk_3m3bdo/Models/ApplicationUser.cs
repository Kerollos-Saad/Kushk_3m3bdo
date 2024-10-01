using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Kushk_3m3bdo.Models
{
	public class ApplicationUser : IdentityUser
	{
		[MaxLength(30)]
		public string? FirstName { get; set; }
		[MaxLength(30)]
		public string ? LastName { get; set; }

		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? ZipCode { get; set; }

		public byte[]? ProfilePic { get; set; }

		[ForeignKey("Wallet")]
		public int? WalletId { get; set; }

		public virtual Wallet? Wallet { get; set; }

		public virtual IEnumerable<UserPurchase>? UserPurchases { get; set; }

	}
}
