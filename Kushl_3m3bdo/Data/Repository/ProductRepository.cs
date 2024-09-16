using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.EntityFrameworkCore;

namespace Kushl_3m3bdo.Data.Repository
{
	public class ProductRepository : GenericRepository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _context;

		public ProductRepository(ApplicationDbContext context) : base(context)
		{
			this._context = context;
		}

		public async Task Update(Product newProduct)
		{
			Product oldProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == newProduct.Id);

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
	}
}
