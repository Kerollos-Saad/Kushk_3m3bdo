using Kushk_3m3bdo.Models;

namespace Kushk_3m3bdo.Data.Repository.IRepository
{
	public interface IProductRepository : IGenericRepository<Product>
	{
		Task Update (Product product);
	}
}
