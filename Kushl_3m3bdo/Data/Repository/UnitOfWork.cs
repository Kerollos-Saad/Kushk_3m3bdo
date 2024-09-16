using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public IGenericRepository<ApplicationUser> Users { get; private set; }
		public IGenericRepository<Category> Categories { get; private set; }
		public IGenericRepository<Product> Products { get; private set; }
		public IGenericRepository<UserPurchase> Purchases { get; private set; }
		public IGenericRepository<Wallet> Wallets { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			this._context = context;

			Users = new GenericRepository<ApplicationUser>(_context);
			Categories = new GenericRepository<Category>(_context);
			Products = new GenericRepository<Product>(_context);
			Purchases = new GenericRepository<UserPurchase>(_context);
			Wallets = new GenericRepository<Wallet>(_context);
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
