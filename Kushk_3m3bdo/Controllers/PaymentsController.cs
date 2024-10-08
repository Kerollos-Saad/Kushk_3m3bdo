﻿using System.Collections;
using System.Numerics;
using System.Security.Claims;
using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.Consts;
using Kushk_3m3bdo.Models.Payments;
using Kushk_3m3bdo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Kushk_3m3bdo.Controllers
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

		public async Task<ApplicationUser> GetCurrentUser()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);

			return applicationUser;
		}

		// Nothing
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Plans(int? walletId)
		{
			ViewData["WalletId"] = walletId;

			ViewData["IsUser"] = false;
			ViewData["HaveWallet"] = false;

			ViewData["IsSubscribeToPlan"] = false;
			ViewData["NextSubscriptionDate"] = false;

			if (User.Identity.IsAuthenticated)
			{
				ViewData["IsUser"] = true;

				if (walletId == null)
				{
					// Charge Wallet For Operating User
					ApplicationUser currentUser = await GetCurrentUser();

					if (currentUser.WalletId != null)
					{
						ViewData["HaveWallet"] = true;
						var wallet = await _unitOfWork.Wallets.GetByIdAsync(currentUser.WalletId.Value);

						ViewData["IsSubscribeToPlan"] = wallet.IsSubscribeToPlan;
						ViewData["NextSubscriptionDate"] = wallet.PlanSubscriptionStartDate.AddMonths(1);
					}
				}
				else
				{
					// Charge Wallet For Another User
					var wallet = await _unitOfWork.Wallets.GetByIdAsync(walletId.Value);

					ViewData["HaveWallet"] = true;
					ViewData["IsSubscribeToPlan"] = wallet.IsSubscribeToPlan;
					ViewData["NextSubscriptionDate"] = wallet.PlanSubscriptionStartDate.AddMonths(1);
				}
			}

			var subscriptionPlans = SubscriptionPlan.FetchPlans();
			return View(subscriptionPlans);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PlanPayment(int planId, int? walletId)
		{
			var userWalletId = walletId;
			if (walletId == null)
			{
				ApplicationUser applicationUser = await GetCurrentUser();
				userWalletId = applicationUser.WalletId.Value;
			}

			var wallet = await _unitOfWork.Wallets.GetByIdAsync(userWalletId.Value);

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

			var paymentPlans = SubscriptionPlan.FetchPlans();
			var userPlan = paymentPlans.Find(p => p.Id == planId);
			Session stripeSession;

			if (walletId == null)
			{
				stripeSession = await _stripePaymentService.CreatePlanCheckoutSession(userPlan);
			}
			else
			{
				stripeSession = await _stripePaymentService.CreatePlanCheckoutSessionAdministration(userPlan, walletId.Value);
			}

			return Redirect(stripeSession.Url);
		}

		[HttpGet]
		public async Task<IActionResult> SpecialPlanPayment(int? walletId, int priceAmount)
		{
			var userWalletId = walletId;
			if (walletId == null)
			{
				ApplicationUser applicationUser = await GetCurrentUser();
				userWalletId = applicationUser.WalletId.Value;
			}

			var wallet = await _unitOfWork.Wallets.GetByIdAsync(userWalletId.Value);

			// User Didn't Purchase a Plan For Last Month |OR| 
			// User Purchase a Plan More Than A Month Ago

			// Starting..

			var userPlan = new ChargeWalletPlan()
			{
				Id = 0,
				Name = "Special Plan",
				Price = priceAmount,
				AdditionalCreditPercentage = 0,
				ImgSrc = "https://cdn-icons-png.flaticon.com/512/5939/5939991.png",
				Options = new []{" "}
			};

			var paymentPlans = SubscriptionPlan.FetchPlans();
			SubscriptionPlan.AddPlan(userPlan);

			Session stripeSession;

			if (walletId == null)
			{
				stripeSession = await _stripePaymentService.CreatePlanCheckoutSession(userPlan);
			}
			else
			{
				stripeSession = await _stripePaymentService.CreatePlanCheckoutSessionAdministration(userPlan, walletId.Value);
			}

			return Redirect(stripeSession.Url);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> CheckoutSuccess(string sessionId, int? planId)
		{
			var sessionService = new SessionService();
			var session = await sessionService.GetAsync(sessionId);

			// Here you can save order and customer details to your database.

			var paymentPlans = SubscriptionPlan.FetchPlans();
			var userPlan = paymentPlans.Find(p => p.Id == planId);

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ApplicationUser applicationUser = await _userRepository.GetById(userId);
			var wallet = await _unitOfWork.Wallets.GetByIdAsync(applicationUser.WalletId.Value);

			if (planId != 0)
			{
				wallet.IsSubscribeToPlan = true;
				wallet.SubscriptionPlanId = userPlan.Id;
				wallet.NumberOfSubscriptionPlans += 1;
				wallet.Amount += userPlan.Price * (1 + (decimal)(userPlan.AdditionalCreditPercentage / 100));
				wallet.PlanSubscriptionStartDate = DateTime.UtcNow;
				wallet.Score += (double)(userPlan.Price / 100);

				wallet.IsDebts = (wallet.Amount < 0);
			}
			else
			{
				wallet.NumberOfSubscriptionPlans += 1;
				wallet.Amount += userPlan.Price;
				wallet.Score += (double)(userPlan.Price / 100);

				wallet.IsDebts = (wallet.Amount < 0);
			}

			await _unitOfWork.Wallets.Update(wallet);
			await _unitOfWork.SaveAsync();

			SubscriptionPlan.RemovePlan(userPlan);

			if (User.IsInRole(Roles.Role_Manager) || User.IsInRole(Roles.Role_Admin) ||
			    User.IsInRole(Roles.Role_SubAdmin))
				return RedirectToAction("WalletX", "Wallets", new { walletId = wallet.Id });
			else
				return RedirectToAction("Index", "Wallets");
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> CheckoutSuccessAdministration(int? planId, int walletId)
		{
			var paymentPlans = SubscriptionPlan.FetchPlans();
			var userPlan = paymentPlans.Find(p => p.Id == planId);

			var wallet = await _unitOfWork.Wallets.GetByIdAsync(walletId);

			if (planId != 0)
			{
				wallet.IsSubscribeToPlan = true;
				wallet.SubscriptionPlanId = userPlan.Id;
				wallet.NumberOfSubscriptionPlans += 1;
				wallet.Amount += userPlan.Price * (1 + (decimal)(userPlan.AdditionalCreditPercentage / 100));
				wallet.PlanSubscriptionStartDate = DateTime.UtcNow;
				wallet.Score += (double)(userPlan.Price / 100);

				wallet.IsDebts = (wallet.Amount < 0);
			}
			else
			{
				wallet.NumberOfSubscriptionPlans += 1;
				wallet.Amount += userPlan.Price;
				wallet.Score += (double)(userPlan.Price / 100);

				wallet.IsDebts = (wallet.Amount < 0);
			}

			await _unitOfWork.Wallets.Update(wallet);
			await _unitOfWork.SaveAsync();

			SubscriptionPlan.RemovePlan(userPlan);

			return RedirectToAction("WalletX", "Wallets", new { walletId = wallet.Id });
		}

	}
}
