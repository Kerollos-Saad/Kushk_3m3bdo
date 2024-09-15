using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class ProductRepository : IProductRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductRepository(ApplicationDbContext context)
		{
			this._context = context;
		}

		public async Task<IEnumerable<Product>> GetAll()
		{
			return await _context.Products.ToListAsync();
		}

		public async Task<IEnumerable<Product>> GetByCategoryId(int categoryId)
		{
			return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
		}

		public async Task<Product> GetById(int id)
		{
			return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Product> GetByUPC(long? productUPC)
		{
			return await _context.Products.FirstOrDefaultAsync(p => p.UPCNumber == productUPC);
		}

		public async Task Insert(Product product)
		{
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();
		}

		public async Task Update(Product newProduct)
		{
			Product oldProduct = await GetById(newProduct.Id);

			if (oldProduct != null)
			{
				oldProduct.Name = newProduct.Name;
				oldProduct.Description = newProduct.Description;
				oldProduct.Price = newProduct.Price;
				oldProduct.Discount = newProduct.Discount;
				oldProduct.UPCNumber = newProduct.UPCNumber;
				oldProduct.Company = newProduct.Company;
				oldProduct.Country = newProduct.Country;
				oldProduct.CategoryId = newProduct.CategoryId;
				oldProduct.ProductImg = newProduct.ProductImg;
			}

			await _context.SaveChangesAsync();
		}

		public async Task Delete(int Id)
		{
			Product oldProduct = await GetById(Id);
			
			_context.Products.Remove(oldProduct);
			await _context.SaveChangesAsync();
		}
	}
}
