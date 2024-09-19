using System.Collections;
using System.Security.Claims;
using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Kushl_3m3bdo.Models.Payments;
using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace Kushl_3m3bdo.Controllers
{
	public class PaymentsController : Controller
	{
        private readonly IApplicationUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripePaymentService _stripePaymentService;

        public PaymentsController(IApplicationUserRepository applicationUserRepository, IUnitOfWork unitOfWork, IStripePaymentService stripePaymentService)
        {
            this._userRepository = applicationUserRepository;
            this._unitOfWork = unitOfWork;
            this._stripePaymentService = stripePaymentService;
        }

        public List<ChargeWalletPlan> fetchPlans()
        {
	        var paymentPlans = new List<ChargeWalletPlan>
	        {
		        new ChargeWalletPlan { Id = 1, Name = "Starter", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 100M, AdditionalCreditPercentage = 10.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Economic Plan"}},
		        new ChargeWalletPlan { Id = 2, Name = "Pro 3bdo", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 200M, AdditionalCreditPercentage = 20.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Get Notifications For Offers", "Free Support"}},
		        new ChargeWalletPlan { Id = 3, Name = "Company", ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png", Price = 1000M, AdditionalCreditPercentage = 25.0, NextSubscription = DateTime.UtcNow.AddMonths(1) , Options = new []{"Get Notifications For Offers", "Free Support", "Free Shipping"}}
	        };
            return paymentPlans;
		}

        public async Task<ApplicationUser> GetCurrentUser()
        {
	        var claimsIdentity = (ClaimsIdentity)User.Identity;
	        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

	        ApplicationUser applicationUser = await _userRepository.GetById(userId);

            return applicationUser;
		}
            

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Plans()
		{
			var currentUser = await GetCurrentUser();
			var wallet = await _unitOfWork.Wallets.GetByIdAsync(currentUser.WalletId.Value);

			ViewData["IsSubscribeToPlan"] = wallet.IsSubscripeToPlan;
			ViewData["NextSubscriptionDate"] = wallet.PlanSubscriptionStrated.AddMonths(1);

			var paymentPlans = fetchPlans();
			return View(paymentPlans);
		}

        [HttpPost]
        public async Task<IActionResult> CheckWalletPlanStatus()
        {
			// Assuming you have a way to get the current user's Wallet, for example:
			var currentUser = await GetCurrentUser();
			var wallet = await _unitOfWork.Wallets.GetByIdAsync(currentUser.WalletId.Value);

            if (wallet == null)
            {
                return Json(new { hasActivePlan = false });
            }

            // Check if the wallet has an active plan
            if (wallet.PlanSubscriptionStrated >  DateTime.UtcNow.AddMonths(1))
            {
                return Json(new { hasActivePlan = true, expiryDate = wallet.PlanSubscriptionStrated.AddMonths(1) });
            }

            return Json(new { hasActivePlan = false });
        }

        [HttpPost]
        public async Task<IActionResult> PlanPayment(int PlanId)
        {
	        var claimsIdentity = (ClaimsIdentity)User.Identity;
	        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

	        ApplicationUser applicationUser = await _userRepository.GetById(userId);
	        var wallet = await _unitOfWork.Wallets.GetByIdAsync(applicationUser.WalletId.Value);

			// Check if User Bought a Plan This Month

			if (wallet.IsSubscripeToPlan)
			{
				if (wallet.PlanSubscriptionStrated.AddMonths(1) > DateTime.UtcNow)
				{
					return PartialView("_HavePlanWarnningPartial", wallet);
				}
			}

            // User Didn't Bought a Plan For Last Month


            // Starting..

            var paymentPlans = fetchPlans();
            var userPlan = paymentPlans.Find(p => p.Id == PlanId);

            var checkoutUrl = _stripePaymentService.CreatePlanCheckoutSession(userPlan);

            return Redirect(checkoutUrl);
        }
	}
}
