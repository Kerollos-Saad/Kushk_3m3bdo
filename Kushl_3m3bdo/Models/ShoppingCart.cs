using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kushl_3m3bdo.Models
{
	public class ShoppingCart
	{
		public int Id { get; set; }
		[Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")]
		public int Count { get; set; }

		//--------------------------------------------------------

		public int ProductId { get; set; }
		[ForeignKey("ProductId")] // Explicitly defines ProductId as the foreign key
		[ValidateNever]
		public virtual Product Product { get; set; }

		//--------------------------------------------------------

		[ForeignKey("ApplicationUser")] // The foreign key is inferred from the Product navigation property. EF determines the foreign key is ProductId
		public string ApplicationUserId { get; set; }
		[ValidateNever]
		public virtual ApplicationUser ApplicationUser { get; set; } // Use Lazy Loading (virtual)

		//--------------------------------------------------------

		[NotMapped] // Temporary & Runtime Data
		public double Price { get; set; }
	}
}
