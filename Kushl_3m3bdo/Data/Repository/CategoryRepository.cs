using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
	{
		private readonly ApplicationDbContext _context;
		public CategoryRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}

		public async Task<Category> GetByIdNullable(int? id)
		{
			return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
		}

		public async Task<bool> CheckUniqueCategoryByName(string name)
		{
			var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
			return category == null;
		}

		public async Task Update(Category newCategory)
		{
			var oldCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == newCategory.Id);
			if (oldCategory != null)
			{
				oldCategory.Name = newCategory.Name;
				oldCategory.Logo = newCategory.Logo;
			}
		}
	}
}
