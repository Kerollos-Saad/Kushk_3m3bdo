using System.Collections;
using System.Numerics;
using System.Security.Claims;
using Kushl_3m3bdo.Data.Repository.IRepository;
using Kushl_3m3bdo.Models;
using Kushl_3m3bdo.Models.Payments;
using Kushl_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Kushl_3m3bdo.Controllers
{
	public class PaymentsController : Controller
	{
        private readonly IApplicationUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripePaymentService _stripePaymentService;

        private readonly ILogger<PaymentsController> _logger;


		public PaymentsController(IApplicationUserRepository applicationUserRepository, IUnitOfWork unitOfWork, IStripePaymentService stripePaymentService, ILogger<PaymentsController> logger)
        {
            this._userRepository = applicationUserRepository;
            this._unitOfWork = unitOfWork;
            this._stripePaymentService = stripePaymentService;
			this._logger = logger;
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

			ViewData["IsSubscribeToPlan"] = wallet.IsSubscribeToPlan;
			ViewData["NextSubscriptionDate"] = wallet.PlanSubscriptionStartDate.AddMonths(1);

			var paymentPlans = fetchPlans();
			return View(paymentPlans);
		}

        [HttpPost]
        public async Task<IActionResult> PlanPayment(int PlanId)
        {
	        var claimsIdentity = (ClaimsIdentity)User.Identity;
	        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

	        ApplicationUser applicationUser = await _userRepository.GetById(userId);
	        var wallet = await _unitOfWork.Wallets.GetByIdAsync(applicationUser.WalletId.Value);

			// Check if User Purchase a Plan This Month

			if (wallet.IsSubscribeToPlan)
			{
				if (wallet.PlanSubscriptionStartDate.AddMonths(1) > DateTime.UtcNow)
				{
					return PartialView("_HavePlanWarningPartial", wallet);
				}
			}

			// User Didn't Purchase a Plan For Last Month |OR| 
			// User Purchase a Plan More Than A Month Ago

			// Starting..

			var paymentPlans = fetchPlans();
            var userPlan = paymentPlans.Find(p => p.Id == PlanId);

            var stripeSession = await _stripePaymentService.CreatePlanCheckoutSession(userPlan);

            return Redirect(stripeSession.Url);
        }

        public async Task<IActionResult> CheckoutSuccess(string sessionId, int planId)
        {
	        var sessionService = new SessionService();
	        var session = await sessionService.GetAsync(sessionId);

			// Here you can save order and customer details to your database.

			var paymentPlans = fetchPlans();
			var userPlan = paymentPlans.Find(p => p.Id == planId);

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);
			var wallet = await _unitOfWork.Wallets.GetByIdAsync(applicationUser.WalletId.Value);

			wallet.IsSubscribeToPlan = true;
			wallet.SubscriptionPlanId = userPlan.Id;
			wallet.NumberOfSubscriptionPlans += 1;
			wallet.Amount += userPlan.Price * (1 + (decimal)(userPlan.AdditionalCreditPercentage / 100));
			wallet.PlanSubscriptionStartDate = DateTime.UtcNow;
			wallet.Score += (double)(userPlan.Price / 100);

			wallet.IsDebts = (wallet.Amount < 0);

			await _unitOfWork.Wallets.Update(wallet);
			await _unitOfWork.SaveAsync();

			return RedirectToAction("Index","Wallets");
        }
	}
}
