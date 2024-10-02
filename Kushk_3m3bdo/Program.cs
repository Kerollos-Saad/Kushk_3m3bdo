using Kushk_3m3bdo.Data;
using Kushk_3m3bdo.Data.Repository;
using Kushk_3m3bdo.Data.Repository.IRepository;
using Kushk_3m3bdo.Models;
using Kushk_3m3bdo.Models.Consts;
using Kushk_3m3bdo.Models.Payments;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Kushk_3m3bdo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

            //builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultUI()
				.AddDefaultTokenProviders();

			builder.Services.AddControllersWithViews();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/LoginSpecial";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});

			// Subscribe Stripe Service By Keys
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

			// Register StripePaymentService
			builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();

			// Identity Injection
			builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
			builder.Services.AddScoped<IIdentityRoleReposssitory, IdentityRoleRepository>();
			
			// Model Injection 
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
			builder.Services.AddScoped<IProductRepository, ProductRepository>();

			// Register Generic Repository
			builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			// Register Unit Of Work
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

			// Handel Cycle Reference At Json Data Cause It's Make an Error at DataTables 
			builder.Services.AddControllersWithViews()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
					options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
				});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Two way to Add Services to container
            //StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}
