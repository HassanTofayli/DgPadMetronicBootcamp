using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCartFrontMetronic.Models;

namespace ShoppingCartFrontMetronic.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ImageUrl> ImageUrls { get; set; }
		public DbSet<ShoppingCartFrontMetronic.Models.Admin> Admin { get; set; }

	}
}
