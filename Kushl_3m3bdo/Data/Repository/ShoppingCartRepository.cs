using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository
{
	public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
	{
		private readonly ApplicationDbContext _context;
		public ShoppingCartRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}

		public async Task Update(ShoppingCart shoppingCart)
		{
			_context.ShoppingCarts.Update(shoppingCart);
		}
	}
}
