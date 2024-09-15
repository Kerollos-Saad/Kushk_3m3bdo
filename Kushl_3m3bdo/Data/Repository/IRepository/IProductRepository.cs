using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IProductRepository
	{
		Task<IEnumerable<Product>> GetAll();
		Task<IEnumerable<Product>> GetByCategoryId(int categoryId);
		Task<Product> GetById(int id);
		Task<Product> GetByUPC(long? productUPC);

		Task Insert (Product product);
		Task Update (Product product);
		Task Delete (int Id);

	}
}
