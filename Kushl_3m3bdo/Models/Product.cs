using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kushl_3m3bdo.Models.AttributeValidation;

namespace Kushl_3m3bdo.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public double Price { get; set; }
		
		[Range(0, 100)] 
		public double Discount { get; set; } = 0.0;

		[UPCValidation]
		public long? UPCNumber { get; set; }

		public string? Company { get; set; }
		public string? Country { get; set; }

		public byte[]? ProductImg { get; set; }

		[ForeignKey("Category")] 
		public int? CategoryId { get; set; }
		public virtual Category? Category { get; set; }

		public virtual IEnumerable<UserPurchase>? Purchases { get; set; }
	}
}
