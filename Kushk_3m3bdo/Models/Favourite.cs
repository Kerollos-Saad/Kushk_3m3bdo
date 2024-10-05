using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Models
{
	[PrimaryKey(nameof(ProductId), nameof(UserId))] // Data Annotations EF Core 7.0
	public class Favourite
	{
		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public virtual Product Product { get; set; }

		public string UserId { get; set; }
		[ForeignKey("UserId")]
		[ValidateNever]
		public virtual ApplicationUser ApplicationUser{ get; set; }

		// Fluent API
		// modelBuilder.Entity<Favourite>()
		// .HasKey(f => new { f.ProductId, f.UserId });
	}
}
