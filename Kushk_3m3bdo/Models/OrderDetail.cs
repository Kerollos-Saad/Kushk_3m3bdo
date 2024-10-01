using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kushk_3m3bdo.Models
{
	public class OrderDetail
	{
		// { User Purchases }
		public int Id { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public double Discount { get; set; }

		//--------------------------------------------------------

		public int OrderHeaderId { get; set; }
		[ForeignKey("OrderHeaderId")]
		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }

		//--------------------------------------------------------

		public int ProductId { get; set; }
		[ForeignKey("ProductId")]
		[ValidateNever]
		public Product Product { get; set; }
	}
}
