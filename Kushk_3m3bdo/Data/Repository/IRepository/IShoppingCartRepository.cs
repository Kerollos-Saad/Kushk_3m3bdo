using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
	{
		Task Update(ShoppingCart shoppingCart);
	}
}
