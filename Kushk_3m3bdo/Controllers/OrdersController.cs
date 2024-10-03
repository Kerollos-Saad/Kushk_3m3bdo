using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.Consts;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Kushk_3m3bdo.Data.Repository;
using Stripe;
using Stripe.Climate;

namespace Kushk_3m3bdo.Controllers
{
	public class OrdersController : Controller
	{
		private readonly IApplicationUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderViewModel OrderViewModel { get; set; }

		public OrdersController(IApplicationUserRepository userRepository, IUnitOfWork unitOfWork)
		{
			this._userRepository = userRepository;
			this._unitOfWork = unitOfWork;
		}
		public async Task<ApplicationUser> GetCurrentUser()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);

			return applicationUser;
		}

		[Authorize]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public async Task<IActionResult> Details(int orderId)
		{
			OrderViewModel = new()
			{
				OrderHeader =
					await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == orderId, new[] { "ApplicationUser" }),
				OrderDetails = await _unitOfWork.OrderDetails.FindAllAsync(d => d.OrderHeaderId == orderId,
					includeProperties: new[] { "Product" })
			};
			return View(OrderViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> UpdateOrderDetail()
		{
			var orderHeaderFromDb = await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == OrderViewModel.OrderHeader.Id);

			orderHeaderFromDb.PhoneNumber = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.PhoneNumber) ? OrderViewModel.OrderHeader.PhoneNumber : null;
			orderHeaderFromDb.StreetAddress = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.StreetAddress) ? OrderViewModel.OrderHeader.StreetAddress : null;
			orderHeaderFromDb.City = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.City) ? OrderViewModel.OrderHeader.City : null;
			orderHeaderFromDb.State = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.State) ? OrderViewModel.OrderHeader.State : null;
			orderHeaderFromDb.ZipCode = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.ZipCode) ? OrderViewModel.OrderHeader.ZipCode : null;
			orderHeaderFromDb.Name = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.Name) ? OrderViewModel.OrderHeader.Name : null;

			orderHeaderFromDb.Carrier = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier) ? OrderViewModel.OrderHeader.Carrier : null;
			orderHeaderFromDb.TrackingNumber = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber) ? OrderViewModel.OrderHeader.TrackingNumber : null;

			await _unitOfWork.OrderHeaders.Update(orderHeaderFromDb);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> StartProcessing()
		{
			await _unitOfWork.OrderHeaders.UpdateStatus(OrderViewModel.OrderHeader.Id, OrderStatus.StatusInProcess);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> ShipOrder()
		{
			var orderHeaderFromDb = await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == OrderViewModel.OrderHeader.Id);

			orderHeaderFromDb.TrackingNumber = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber) ? OrderViewModel.OrderHeader.TrackingNumber : null;
			orderHeaderFromDb.Carrier = !String.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier) ? OrderViewModel.OrderHeader.Carrier : null;

			orderHeaderFromDb.OrderStatus = OrderStatus.StatusShipped;
			orderHeaderFromDb.ShippingDate = DateTime.UtcNow;



			await _unitOfWork.OrderHeaders.Update(orderHeaderFromDb);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Roles.Role_Manager + "," + Roles.Role_Admin + "," + Roles.Role_SubAdmin)]
		public async Task<IActionResult> CancelOrder()
		{
			var orderHeaderFromDb = await _unitOfWork.OrderHeaders.FindAsync(h => h.Id == OrderViewModel.OrderHeader.Id);

			if (orderHeaderFromDb.PaymentStatus == PaymentStatus.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeaderFromDb.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = await service.CreateAsync(options);

				await _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.StatusCancelled,
					PaymentStatus.PaymentStatusRefunded);
			}
			else
			{
				await _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.StatusCancelled,
					PaymentStatus.PaymentStatusCancelled);
			}

			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		#region DATATABLES 

		[HttpGet]
		public async Task<IActionResult> GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders;

			if (User.IsInRole(Roles.Role_Manager) || User.IsInRole(Roles.Role_Admin))
			{
				objOrderHeaders =
					await _unitOfWork.OrderHeaders.FindAllAsync(includeProperties: new[] { "ApplicationUser" });
			}
			else
			{
				var currentUser = await GetCurrentUser();
				objOrderHeaders = await _unitOfWork.OrderHeaders.FindAllAsync(
					h => h.ApplicationUserId == currentUser.Id, null, null, new[] { "ApplicationUser" });
			}

			switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == PaymentStatus.PaymentStatusPending);
					break;
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == OrderStatus.StatusInProcess);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == OrderStatus.StatusShipped);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == OrderStatus.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = objOrderHeaders.ToList() });
		}

		#endregion


	}

}
