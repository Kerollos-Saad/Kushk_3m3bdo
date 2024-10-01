using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Data.Repository
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}

		public async Task Update(OrderHeader orderHeader)
		{
			_context.OrderHeaders.Update(orderHeader);
		}

		public async Task UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
			if (!String.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
			}
			if(!String.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.UtcNow;
			}
		}

		public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if (!String.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}
	}
}
