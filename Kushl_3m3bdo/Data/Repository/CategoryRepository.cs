using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly ApplicationDbContext _context;
		public CategoryRepository(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<Category>> GetAll()
		{
			return await _context.Categories.ToListAsync();
		}

		public async Task<Category> GetById(int id)
		{
			return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
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

		public async Task Insert(Category newCategory)
		{
			await _context.Categories.AddAsync(newCategory);
			await _context.SaveChangesAsync();
		}

		public async Task Update(Category newCategory)
		{
			var oldCategory = await GetById(newCategory.Id);
			if (oldCategory != null)
			{
				oldCategory.Name = newCategory.Name;
				oldCategory.Logo = newCategory.Logo;
			}
			await _context.SaveChangesAsync();
		}

		public async Task Delete(int Id)
		{
			Category oldCategory = await GetById(Id);


			_context.Categories.Remove(oldCategory);
			await _context.SaveChangesAsync();
		}
	}
}
