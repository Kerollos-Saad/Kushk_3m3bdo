using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class WalletRepository : IWalletRepository
	{
		private readonly ApplicationDbContext _context;

		public WalletRepository(ApplicationDbContext context)
		{
			this._context = context;
		}

		public IEnumerable<Wallet> GetAll()
		{
			return _context.Wallets.ToList();
		}

		public Wallet GetById(int id)
		{
			return _context.Wallets.FirstOrDefault(w => w.Id == id);
		}

		public void Insert(Wallet wallet)
		{
			_context.Wallets.Add(wallet);
			_context.SaveChanges();
		}

		public void Update(int Id, Wallet newWallet)
		{
			Wallet oldWallet = GetById(Id);
			if (oldWallet != null)
			{
				oldWallet.Amount = newWallet.Amount;
				oldWallet.isDebts = newWallet.isDebts;
				oldWallet.ApplicationUserId = newWallet.ApplicationUserId;
			}
			_context.SaveChanges();
		}

		public void Delete(int Id)
		{
			Wallet oldWallet = GetById(Id);

			_context.Wallets.Remove(oldWallet);
			_context.SaveChanges();
			
		}
	}
}
