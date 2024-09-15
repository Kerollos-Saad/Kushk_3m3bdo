using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Kushl_3m3bdo.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly ICategoryRepository _categoryRepository;

		public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
		{
			this._productRepository = productRepository;
			this._categoryRepository = categoryRepository;
		}

		//[Authorize(Roles = "User,Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Index(string searchString)
		{
            ViewData["CategoryList"] = await _categoryRepository.GetAll();

            var products = await _productRepository.GetAll();

            if (!String.IsNullOrEmpty(searchString))
	            products = products.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));

			return View(products);
		}

		[HttpGet]
		[Authorize(Roles = "Admin,SubAdmin,Manager")]
        public async Task<IActionResult> Add()
        {
	        ViewData["CategoryList"] = await _categoryRepository.GetAll();

			return View(new Product());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product newProduct)
        {
	        ViewData["CategoryList"] = await _categoryRepository.GetAll();

			if (ModelState.IsValid)
	        {
		        if (newProduct.UPCNumber != null)
		        {
			        var oldProduct = await _productRepository.GetByUPC(newProduct.UPCNumber);

			        if (oldProduct != null)
			        {
				        ModelState.AddModelError("RedundantProduct", "Product UPC is Already Exist!");
				        return View("Add", newProduct);
			        }
		        }

		        var productImg = Request.Form.Files.FirstOrDefault();
		        if (productImg != null)
		        {
			        using (var dataStream = new MemoryStream())
			        {
				        await productImg.CopyToAsync(dataStream);
						newProduct.ProductImg = dataStream.ToArray();
			        }
		        }

		        await _productRepository.Insert(newProduct);

		        return RedirectToAction(nameof(Index));
			}
	        else
	        {
		        return View("Add", newProduct);
	        }

		}

		[HttpGet]
		//[Authorize(Roles = "User,Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Details(int Id)
        {
	        var targetProduct = await _productRepository.GetById(Id);

			var Category = await _categoryRepository.GetByIdNullable(targetProduct.CategoryId);
			ViewData["ProductCategory"] = Category.Name;

			return View(targetProduct);
        }

		[HttpGet]
		[Authorize(Roles = "Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Update(int ProductId)
		{
			ViewData["CategoryList"] = await _categoryRepository.GetAll();
			var targetProduct = await _productRepository.GetById(ProductId);
			return View(targetProduct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
        public async Task<IActionResult> Update(Product newProduct)
        {
            ViewData["CategoryList"] = await _categoryRepository.GetAll();

            if (!ModelState.IsValid)
                return View(newProduct);

            var oldProduct = await _productRepository.GetById(newProduct.Id);

            var image = Request.Form.Files.FirstOrDefault();
            if (image != null)
            {
                using (var dataStream = new MemoryStream())
                {
                    await image.CopyToAsync(dataStream);
                    newProduct.ProductImg = dataStream.ToArray();
                }
            }
            else
            {
                newProduct.ProductImg = oldProduct.ProductImg;
            }

            if (oldProduct.Name == newProduct.Name &&
                oldProduct.Description == newProduct.Description &&
                oldProduct.Price == newProduct.Price &&
                oldProduct.Discount == newProduct.Discount &&
                oldProduct.UPCNumber == newProduct.UPCNumber &&
                oldProduct.Company == newProduct.Company &&
                oldProduct.Country == newProduct.Country &&
                oldProduct.CategoryId == newProduct.CategoryId &&
                oldProduct.ProductImg == newProduct.ProductImg)
            {
                ModelState.AddModelError("NoChanges", "No Changes on Product!");
                return View(newProduct);
            }

            if (oldProduct.UPCNumber != newProduct.UPCNumber)
            {
                var uniqueUPC = await _productRepository.GetByUPC(newProduct.UPCNumber);
                if (uniqueUPC != null)
                {
                    ModelState.AddModelError("RedundantUPC", "UPC Number is Already Exist!");
                    return View(newProduct);
                }
            }

            await _productRepository.Update(newProduct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(int categoryId)
        {

			var TargetProducts = await _productRepository.GetByCategoryId(categoryId);
			return View("Index",TargetProducts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Remove(int ProductId)
        {
	        await _productRepository.Delete(ProductId);
			return RedirectToAction(nameof(Index));
        }

	}
}
