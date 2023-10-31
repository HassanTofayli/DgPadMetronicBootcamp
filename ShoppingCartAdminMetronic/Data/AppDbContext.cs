using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAdminMetronic.Models;

namespace ShoppingCartAdminMetronic.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ImageUrl> ImageUrls { get; set; }
		public DbSet<ShoppingCartAdminMetronic.Models.Admin> Admin { get; set; }

	}
}
