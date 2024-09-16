using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IUserPurchaseRepository : IGenericRepository<UserPurchase>
	{
		Task<IEnumerable<UserPurchase>> GetByUserId(String Id);

		Task Update (UserPurchase userPurchase);
	}
}
