using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface ICategoryRepository : IGenericRepository<Category>
	{
		Task<Category> GetByIdNullable(int? id);
		Task<bool> CheckUniqueCategoryByName(string name);

		Task Update(Category category);
	}
}
