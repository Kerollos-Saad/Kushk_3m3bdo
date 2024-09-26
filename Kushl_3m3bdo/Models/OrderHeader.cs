using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kushl_3m3bdo.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }

		public DateTime OrderDate { get; set; }
		public double OrderTotal { get; set; }

		public String? OrderStatus { get; set; }
		public String? PaymentStatus { get; set; }
		public String? TrackingNumber { get; set; }
		public String? Carrier { get; set; }

		public DateTime? ShippingDate { get; set; }

		public String? PaymentType { get; set; } // Wallet | Credit
		public String? OrderType { get; set; } // Delivery | Pickup

		public DateTime PaymentDate { get; set; }
		public DateTime PaymentDueDate { get; set; }

		public String? SessionId { get; set; }
		public String? PaymentIntentId { get; set; }

		//--------------------------------------------------------
		public string? SalesId { get; set; }
		[ForeignKey("SalesId")]
		[ValidateNever]
		public virtual ApplicationUser? SalesApplicationUser { get; set; } 

		//--------------------------------------------------------
		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		//--------------------------------------------------------
		public String? PhoneNumber { get; set; }
		public String? StreetAddress { get; set; }
		public String? City { get; set; }
		public String? State { get; set; }
		public String? ZipCode { get; set;}
		public String? Name { get; set; }
	}
}
