using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IProductRepository : IGenericRepository<Product>
	{
		Task Update (Product product);
	}
}
