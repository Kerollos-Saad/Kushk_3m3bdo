using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
	{
		Task Update(OrderHeader orderHeader);
		Task UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
		Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
	}
}
