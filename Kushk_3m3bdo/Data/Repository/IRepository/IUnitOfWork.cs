using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IUnitOfWork : IDisposable
	{
		ICategoryRepository Categories { get; }
		IProductRepository Products { get; }
		IWalletRepository Wallets { get; }
		IShoppingCartRepository ShoppingCarts { get; }
		IOrderHeaderRepository OrderHeaders { get; }
		IOrderDetailRepository OrderDetails { get; }

		IFavouriteRepository Favorites { get; }
		IDebitRepository Debits { get; }

		void Save();
		Task SaveAsync();

	}
}
