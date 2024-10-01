using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Data.Repository
{
    public class UserPurchaseRepository : GenericRepository<UserPurchase>, IUserPurchaseRepository
    {

		private readonly ApplicationDbContext _context;

		public UserPurchaseRepository(ApplicationDbContext context):base(context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<UserPurchase>> GetByUserId(String Id)
	    {
		    return await _context.UserPurchases.Where(p => p.ApplicationUserId == Id).ToListAsync();
	    }


	    public async Task Update(UserPurchase newUserPurchase)
	    {
		    UserPurchase oldUserPurchase = await _context.UserPurchases.FirstOrDefaultAsync(p => p.Id == newUserPurchase.Id);
			if (oldUserPurchase != null)
		    {
				oldUserPurchase.Quantity = newUserPurchase.Quantity;
				oldUserPurchase.PurchaseTime = newUserPurchase.PurchaseTime;
		    }
	    }

	}
}
