using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Kushk_3m3bdo.Models.Consts;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kushk_3m3bdo.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IApplicationUserRepository _userRepository;

		public ProductsController(IUnitOfWork unitOfWork, IApplicationUserRepository userRepository)
		{
			this._unitOfWork = unitOfWork;
			this._userRepository = userRepository;
		}

		private async Task<ApplicationUser> GetCurrentUser()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);

			return applicationUser;
		}

		public async Task<IActionResult> Index(string searchString)
		{
			ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			var products = await _unitOfWork.Products.FindAllAsync(p => !p.IsDeleted);

            if (!String.IsNullOrEmpty(searchString))
	            products = products.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));

			return View(products);
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> Add()
        {
	        ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			return View(new Product()); // new product object created server-side So no need to set to 0 after post
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
        public async Task<IActionResult> Add(Product newProduct)
        {
	        ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			if (ModelState.IsValid)
	        {
		        if (newProduct.UPCNumber != null)
		        {
			        var oldProduct =
				        await _unitOfWork.Products.FindAsync(p => p.UPCNumber == newProduct.UPCNumber);

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
		        await _unitOfWork.SaveAsync();
		        return RedirectToAction(nameof(Index));
			}
	        else
	        {
		        return View("Add", newProduct);
	        }

		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Details(int id)
		{
			TempData["ProductError"] = TempData["ProductError"];

			ShoppingCart cart = new()
			{
				Product = await _unitOfWork.Products.FindAsync(p => p.Id == id, new[] { "Category" }),
				ProductId = id,
				Quantity = 1
			};

			return View(cart);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Details(ShoppingCart shoppingCart)
		{
			var product = await _unitOfWork.Products.FindAsync(p => p.Id == shoppingCart.ProductId);
			if (product.IsDeleted)
			{
				TempData["ProductError"] = "This Product Was Deleted!";
				return RedirectToAction(nameof(Details), routeValues: shoppingCart.ProductId);
			}


			ApplicationUser currentUser = await GetCurrentUser();

			shoppingCart.Id = 0; // ensures it's a new record to generate new Id Cause it's return from web form
			shoppingCart.ApplicationUserId = currentUser.Id;

			ShoppingCart cartFromDb = await _unitOfWork.ShoppingCarts.FindAsync(c =>
				c.ApplicationUserId == currentUser.Id &&
				c.ProductId == shoppingCart.ProductId);

			if (cartFromDb != null)
			{
				cartFromDb.Quantity += shoppingCart.Quantity;
				await _unitOfWork.ShoppingCarts.Update(cartFromDb);
			}
			else
			{
				TempData["CartCount"] = (int)TempData.Peek("CartCount") + 1;
				await _unitOfWork.ShoppingCarts.AddAsync(shoppingCart);
			}

			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index), "Carts");
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> AddToCart(int id, int quantity)
		{

			var product = await _unitOfWork.Products.FindAsync(p => p.Id == id, new[] { "Category" });
			if (product.IsDeleted)
			{
				TempData["ProductError"] = "Can't Buy Deleted Products!";
				return RedirectToAction(nameof(RemovedProducts));
			}

			ShoppingCart cart = new()
			{
				Product = product,
				ProductId = id,
				Quantity = quantity
			};

			ApplicationUser currentUser = await GetCurrentUser();

			cart.Id = 0; // ensures it's a new record to generate new Id Cause it's return from web form
			cart.ApplicationUserId = currentUser.Id;

			ShoppingCart cartFromDb = await _unitOfWork.ShoppingCarts.FindAsync(c =>
				c.ApplicationUserId == currentUser.Id &&
				c.ProductId == cart.ProductId);

			if (cartFromDb != null)
			{
				cartFromDb.Quantity += cart.Quantity;
				await _unitOfWork.ShoppingCarts.Update(cartFromDb);
			}
			else
			{
				TempData["CartCount"] = (int)TempData.Peek("CartCount") + 1;
				await _unitOfWork.ShoppingCarts.AddAsync(cart);
			}

			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index), "Carts");

		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> AddFavourite(int id)
		{
			ApplicationUser currentUser = await GetCurrentUser();

			return Json(id.ToString() + currentUser.Id.ToString());

		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> Update(int ProductId)
		{
			ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();
			var targetProduct = await _unitOfWork.Products.GetByIdAsync(ProductId);
			return View(targetProduct);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
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
		            await _unitOfWork.Products.FindAsync(p => p.UPCNumber == newProduct.UPCNumber);

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
		[Authorize]
        public async Task<IActionResult> FilterByCategory(int categoryId)
        {
	        ViewData["CategoryList"] = await _unitOfWork.Categories.GetAllAsync();

			var TargetProducts =
		        await _unitOfWork.Products.FindAllExpressionPropAsync(p => p.CategoryId == categoryId && !p.IsDeleted);


			return View("Index",TargetProducts);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> Remove(int productId)
		{
			// Hard Remove Can't Process Cause REFERENCE constraint "FK_OrderDetails_Products_ProductId"
			//await _unitOfWork.Products.RemoveByIdAsync(ProductId);
			//await _unitOfWork.SaveAsync();

			// Soft Delete
			var targetProduct = await _unitOfWork.Products.FindAsync(p => p.Id == productId);
			if (targetProduct != null && targetProduct.IsDeleted == false)
			{
				targetProduct.IsDeleted = true;
				await _unitOfWork.SaveAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return NotFound();
			}
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> ReBack(int productId)
		{
			var targetProduct = await _unitOfWork.Products.FindAsync(p => p.Id == productId);
			if (targetProduct != null && targetProduct.IsDeleted == true)
			{
				targetProduct.IsDeleted = false;
				await _unitOfWork.SaveAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				return NotFound();
			}
		}

		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin)]
		public async Task<IActionResult> RemovedProducts()
		{
			var targetProducts = await _unitOfWork.Products.FindAllAsync(p => p.IsDeleted);
			return View(nameof(Index), targetProducts);
		}


		#region DataTables API CALLS 
		// Cancelled
		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager)]
		public async Task<IActionResult> ProductInfo()
		{
			//var products = await _unitOfWork.Products.FindAllAsync(null, null, null, new[] { "Category" });
			//var productObj = products.Select(p => new
			//{
			//	p.Id, 
			//	p.Name, 
			//	p.Description, 
			//	p.Price, 
			//	p.Discount,
			//	upcNumber = p.UPCNumber.ToString(),
			//	p.Company, 
			//	p.Country,
			//	categoryId = p.Category.Id,
			//	categoryName = p.Category.Name
			//}).ToList();

			return View();
		}
		[HttpGet]
		[Authorize(Roles = Roles.Role_Manager)]
		public async Task<IActionResult> GetAll()
		{
			var products = await
				_unitOfWork.Products.FindAllAsync(null, null, null, new[] { "Category" });

			//return Json(products);
			return Json(new { data = products });
		}

		#endregion


	}
}
