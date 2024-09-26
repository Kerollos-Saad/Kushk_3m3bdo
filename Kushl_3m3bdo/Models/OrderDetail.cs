using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kushl_3m3bdo.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }
		public int Count { get; set; }
		public decimal Price { get; set; }

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
