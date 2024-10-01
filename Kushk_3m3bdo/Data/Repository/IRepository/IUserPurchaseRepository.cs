using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IUserPurchaseRepository : IGenericRepository<UserPurchase>
	{
		Task<IEnumerable<UserPurchase>> GetByUserId(String Id);

		Task Update (UserPurchase userPurchase);
	}
}
