using System.Drawing.Printing;
using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kushk_3m3bdo.Controllers
{
	[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
	public class CategoriesController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;

		public CategoriesController(IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
		}

		private async Task<Dictionary<int, int>> CreateProductCategoryDict()
		{
			Dictionary<int, int> ProductNumPerCategory = new Dictionary<int, int>();

			var Products = await _unitOfWork.Products.GetAllAsync();
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

			var categories = await _unitOfWork.Categories.GetAllAsync();
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

			bool UniqueName = await _unitOfWork.Categories.CheckUniqueCategoryByName(newCategory.Name);

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

			await _unitOfWork.Categories.AddAsync(newCategory);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Remove(int categoryId)
		{
			await _unitOfWork.Categories.RemoveByIdAsync(categoryId);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Update(int categoryId)
		{
			var TargetCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);
			return View(TargetCategory);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(Category newCategory)
		{
			if (!ModelState.IsValid)
				return View(newCategory);

			var oldCategory = await _unitOfWork.Categories.GetByIdAsync(newCategory.Id);
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
				bool UniqueName = await _unitOfWork.Categories.CheckUniqueCategoryByName(newCategory.Name);

				if (!UniqueName)
				{
					ModelState.AddModelError("RedundantName", "Category is Already Exist!");
					return View(newCategory);
				}
			}

			await _unitOfWork.Categories.Update(newCategory);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Pagination(int page = 1, int pageSize = 4)
		{
			var paginatedCategories = await _unitOfWork.Categories.FindAllAsync(null, (page - 1) * pageSize, pageSize);
			ViewData["TotalPages"] = (int)Math.Ceiling((double)_unitOfWork.Categories.GetAll().Count() / pageSize);
			ViewData["CurrentPage"] = page;

			//return View(paginatedCategories.ToList());

			return View(await _unitOfWork.Categories.FindAllAsync());
		}


	}
}
