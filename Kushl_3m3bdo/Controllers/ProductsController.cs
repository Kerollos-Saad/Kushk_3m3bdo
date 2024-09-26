using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Kushl_3m3bdo.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductsController(IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
		}

		//[Authorize(Roles = "User,Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Index(string searchString)
		{
			ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			var products = await _unitOfWork.Products.GetAllAsync();

            if (!String.IsNullOrEmpty(searchString))
	            products = products.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));

			return View(products);
		}

		[HttpGet]
		[Authorize(Roles = "Admin,SubAdmin,Manager")]
        public async Task<IActionResult> Add()
        {
	        ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			return View(new Product());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product newProduct)
        {
	        ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			if (ModelState.IsValid)
	        {
		        if (newProduct.UPCNumber != null)
		        {
			        var oldProduct =
				        await _unitOfWork.Products.FindExpressionAsync(p => p.UPCNumber == newProduct.UPCNumber);

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

		        await _unitOfWork.Products.AddAsync(newProduct);
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
			// First Ways
			//var targetProduct = await _unitOfWork.Products.GetByIdAsync(Id);

			//var Category = await _unitOfWork.Categories.GetByIdNullable(targetProduct.CategoryId);
	        
			//ViewData["ProductCategory"] = Category.Name;

			// Second Way
	        var target = await _unitOfWork.Products.FindExpressionAsync(p => p.Id == Id,new[] { "Category" });
	        ViewData["ProductCategory"] = target.Category.Name;

			return View(target);
        }

		[HttpGet]
		[Authorize(Roles = "Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Update(int ProductId)
		{
			ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();
			var targetProduct = await _unitOfWork.Products.GetByIdAsync(ProductId);
			return View(targetProduct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
        public async Task<IActionResult> Update(Product newProduct)
        {
            ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			if (!ModelState.IsValid)
                return View(newProduct);

            var oldProduct = await _unitOfWork.Products.GetByIdAsync(newProduct.Id);

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
	            var uniqueUPC =
		            await _unitOfWork.Products.FindExpressionAsync(p => p.UPCNumber == newProduct.UPCNumber);

                if (uniqueUPC != null)
                {
                    ModelState.AddModelError("RedundantUPC", "UPC Number is Already Exist!");
                    return View(newProduct);
                }
            }

            await _unitOfWork.Products.Update(newProduct);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(int categoryId)
        {
			var TargetProducts = await _unitOfWork.Products.FindAllExpressionAsync(p => p.CategoryId == categoryId);
			return View("Index",TargetProducts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SubAdmin,Manager")]
		public async Task<IActionResult> Remove(int ProductId)
		{
			await _unitOfWork.Products.RemoveWithIdAsync(ProductId);
			return RedirectToAction(nameof(Index));
        }

	}
}
