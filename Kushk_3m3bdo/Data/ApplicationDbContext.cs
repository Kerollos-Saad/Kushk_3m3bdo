using Kushk_3m3bdo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kushk_3m3bdo.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>()
				.ToTable("Users", "Security");
			builder.Entity<IdentityRole>()
				.ToTable("Roles", "security");
			builder.Entity<IdentityUserRole<String>>()
				.ToTable("UserRoles", "security");
			builder.Entity<IdentityUserClaim<String>>()
				.ToTable("UserClaims", "security");
			builder.Entity<IdentityUserLogin<String>>()
				.ToTable("UserLogins", "security");
			builder.Entity<IdentityRoleClaim<String>>()
				.ToTable("RoleClaims", "security");
			builder.Entity<IdentityUserToken<String>>()
				.ToTable("UserTokens", "security");
		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Wallet> Wallets { get; set; }

		public DbSet<ShoppingCart> ShoppingCarts { get; set; }
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }

		public DbSet<Favourite> Favorites { get; set; }
		public DbSet<Debit> Debits { get; set; }
	}
}
