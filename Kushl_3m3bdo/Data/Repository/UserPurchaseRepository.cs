using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
    public class UserPurchaseRepository : IUserPurchaseRepository
    {

		private readonly ApplicationDbContext _context;

		public UserPurchaseRepository(ApplicationDbContext context)
		{
			this._context = context;
		}

		public UserPurchase GetById(int Id)
		{
			return _context.UserPurchases.FirstOrDefault(p => p.Id == Id);
		}

		public IEnumerable<UserPurchase> GetByUserId(String Id)
	    {
		    return _context.UserPurchases.Where(p => p.ApplicationUserId == Id).ToList();
	    }

	    public void Insert(UserPurchase userPurchase)
	    {
		    _context.UserPurchases.Add(userPurchase);
			_context.SaveChanges();
	    }

	    public void Update(int Id, UserPurchase newUserPurchase)
	    {
		    UserPurchase oldUserPurchase = GetById(Id);
		    if (oldUserPurchase != null)
		    {
				oldUserPurchase.Quantity = newUserPurchase.Quantity;
				oldUserPurchase.PurchaseTime = newUserPurchase.PurchaseTime;
		    }
			_context.SaveChanges();
	    }

	    public void Delete(int Id)
	    {
			UserPurchase oldUserPurchase = GetById(Id);

			_context.UserPurchases.Remove(oldUserPurchase);
			_context.SaveChanges();
	    }
	}
}
