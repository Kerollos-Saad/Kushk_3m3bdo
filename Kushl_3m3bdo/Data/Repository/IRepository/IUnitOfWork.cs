using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IUnitOfWork : IDisposable
	{
		IGenericRepository<ApplicationUser> Users { get; }
		IGenericRepository<Category> Categories { get; }
		IGenericRepository<Product> Products { get; }
		IGenericRepository<UserPurchase> Purchases { get; }
		IGenericRepository<Wallet> Wallets { get; }

		void Save();
		void SaveAsync();

	}
}
