using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IUserPurchaseRepository
	{
		UserPurchase GetById (int Id);
		IEnumerable<UserPurchase> GetByUserId(String Id);

		void Insert (UserPurchase userPurchase);
		void Update (int Id, UserPurchase userPurchase);
		void Delete (int Id);
	}
}
