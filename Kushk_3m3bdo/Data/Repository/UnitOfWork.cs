using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public ICategoryRepository Categories { get; private set; }
		public IProductRepository Products { get; private set; }
		public IWalletRepository Wallets { get; private set; }
		public IShoppingCartRepository ShoppingCarts { get; private set; }
		public IOrderHeaderRepository OrderHeaders { get; private set; }
		public IOrderDetailRepository OrderDetails { get; private set; }
		public IFavouriteRepository Favorites { get; private set; }
		public IDebitRepository Debits { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			this._context = context;

			Categories = new CategoryRepository(_context);
			Products = new ProductRepository(_context);
			Wallets = new WalletRepository(_context);
			ShoppingCarts = new ShoppingCartRepository(_context);
			OrderHeaders = new OrderHeaderRepository(_context);
			OrderDetails = new OrderDetailRepository(_context);
			Favorites = new FavouriteRepository(_context);
			Debits = new DebitRepository(_context);
		}

		public void Save()
		{
			_context.SaveChanges();
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
