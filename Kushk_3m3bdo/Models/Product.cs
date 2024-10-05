using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kushk_3m3bdo.Models.AttributeValidation;

namespace Kushk_3m3bdo.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public string? ProductImgUrl { get; set; }
		public decimal Price { get; set; }

		[Range(0, 100, ErrorMessage = "Discount Should Between 0~100%")]
		public double Discount { get; set; } = 0.0;

		[UPCValidation]
		public long? UPCNumber { get; set; }

		public string? Company { get; set; }
		public string? Country { get; set; }

		[Required]
		[Range(0,50000, ErrorMessage = "Stock Can't be 0 or Negative")]
		public int Stock { get; set; }

		public byte[]? ProductImg { get; set; }
		public bool IsDeleted { get; set; } = false;


		[ForeignKey("Category")] 
		public int? CategoryId { get; set; }
		public virtual Category? Category { get; set; }
	}
}
