using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public ICategoryRepository Categories { get; private set; }
		public IProductRepository Products { get; private set; }
		public IUserPurchaseRepository Purchases { get; private set; }
		public IWalletRepository Wallets { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			this._context = context;

			Categories = new CategoryRepository(_context);
			Products = new ProductRepository(_context);
			Purchases = new UserPurchaseRepository(_context);
			Wallets = new WalletRepository(_context);
		}

		public void Save()
		{
			_context.SaveChanges();
		}

		public void SaveAsync()
		{
			_context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
