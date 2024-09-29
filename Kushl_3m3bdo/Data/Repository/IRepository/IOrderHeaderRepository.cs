using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
	{
		Task UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
		Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
	}
}
