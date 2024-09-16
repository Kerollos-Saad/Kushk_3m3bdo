using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IUnitOfWork : IDisposable
	{
		ICategoryRepository Categories { get; }
		IProductRepository Products { get; }
		IUserPurchaseRepository Purchases { get; }
		IWalletRepository Wallets { get; }

		void Save();
		void SaveAsync();

	}
}
