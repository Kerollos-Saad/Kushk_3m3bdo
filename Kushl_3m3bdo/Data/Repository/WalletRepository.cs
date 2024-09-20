using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
	{
		private readonly ApplicationDbContext _context;

		public WalletRepository(ApplicationDbContext context):base(context)
		{
			this._context = context;
		}

		public async Task Update(Wallet newWallet)
		{
			Wallet oldWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == newWallet.Id);
			if (oldWallet != null)
			{
				oldWallet.Amount = newWallet.Amount;
				oldWallet.IsDebts = newWallet.IsDebts;
				oldWallet.DebtRequest = newWallet.DebtRequest;
				
				oldWallet.SubscriptionPlanId = newWallet.SubscriptionPlanId;
				oldWallet.IsSubscribeToPlan = newWallet.IsSubscribeToPlan;
				oldWallet.NumberOfSubscriptionPlans = newWallet.NumberOfSubscriptionPlans;
				oldWallet.PlanSubscriptionStartDate = newWallet.PlanSubscriptionStartDate;
				oldWallet.WalletCreatedDate = newWallet.WalletCreatedDate;
				oldWallet.NumberOfPurchases = newWallet.NumberOfPurchases;
				oldWallet.PriceOfPurchases = newWallet.PriceOfPurchases;
				oldWallet.Score = newWallet.Score;

				oldWallet.ApplicationUserId = newWallet.ApplicationUserId;
			}
		}
	}
}
