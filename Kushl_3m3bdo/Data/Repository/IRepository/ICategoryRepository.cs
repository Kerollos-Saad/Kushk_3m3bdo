using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAll();
		Task<Category> GetById(int id);
		Task<Category> GetByIdNullable(int? id);
		Task<bool> CheckUniqueCategoryByName(string name);

		Task Insert(Category category);
		Task Update(Category category);
		Task Delete(int Id);
	}
}
