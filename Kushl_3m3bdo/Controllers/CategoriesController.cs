using System.Drawing.Printing;
using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kushl_3m3bdo.Controllers
{
	[Authorize(Roles = "Manager,Admin")]
	public class CategoriesController : Controller
	{

		private readonly ICategoryRepository _categoryRepository;
		private readonly IProductRepository _productRepository;

		public CategoriesController(ICategoryRepository categoryRepository, IProductRepository productRepository)
		{
			this._categoryRepository = categoryRepository;
			this._productRepository = productRepository;
		}

		public async Task<Dictionary<int, int>> CreateProductCategoryDict()
		{
			Dictionary<int, int> ProductNumPerCategory = new Dictionary<int, int>();

			var Products = await _productRepository.GetAll();
			foreach (Product product in Products)
			{
				if (ProductNumPerCategory.ContainsKey((int)product.CategoryId))
					ProductNumPerCategory[(int)product.CategoryId] += 1;
				else
					ProductNumPerCategory[(int)product.CategoryId] = 1;
			}

			return ProductNumPerCategory;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			ViewData["ProductsPerCategory"] = await CreateProductCategoryDict();

			var categories = await _categoryRepository.GetAll();
			return View(categories);
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			return View(new Category());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(Category newCategory)
		{

			bool UniqueName = await _categoryRepository.CheckUniqueCategoryByName(newCategory.Name);

			if (!UniqueName)
			{
				ModelState.AddModelError("RedundantName", "Category is Already Exist!");
				return View("Add", newCategory);
			}

			var image = Request.Form.Files.FirstOrDefault();

			if (image != null)
			{
				using (var dataStream = new MemoryStream())
				{
					await image.CopyToAsync(dataStream);
					newCategory.Logo = dataStream.ToArray();
				}
			}

			await _categoryRepository.Insert(newCategory);

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Remove(int categoryId)
		{
			await _categoryRepository.Delete(categoryId);

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Update(int categoryId)
		{
			var TargetCategory = await _categoryRepository.GetById(categoryId);
			return View(TargetCategory);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(Category newCategory)
		{
			if (!ModelState.IsValid)
				return View(newCategory);

			var oldCategory = await _categoryRepository.GetById(newCategory.Id);
			if (oldCategory == null)
			{
				ModelState.AddModelError("NotFound", "Category not found.");
				return View(newCategory);
			}

			var image = Request.Form.Files.FirstOrDefault();
			if (image != null)
			{
				using (var dataStream = new MemoryStream())
				{
					await image.CopyToAsync(dataStream);
					newCategory.Logo = dataStream.ToArray();
				}
			}
			else
			{
				newCategory.Logo = oldCategory.Logo;
			}

			if (oldCategory.Name == newCategory.Name && oldCategory.Logo == newCategory.Logo)
			{
				ModelState.AddModelError("NoChanges", "No Changes on Category!");
				return View(newCategory);
			}

			if (oldCategory.Name != newCategory.Name)
			{
				bool UniqueName = await _categoryRepository.CheckUniqueCategoryByName(newCategory.Name);

				if (!UniqueName)
				{
					ModelState.AddModelError("RedundantName", "Category is Already Exist!");
					return View(newCategory);
				}
			}

			await _categoryRepository.Update(newCategory);
			return RedirectToAction(nameof(Index));
		}
	}
}
