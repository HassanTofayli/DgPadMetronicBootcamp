using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAdminMetronic.Models;

namespace ShoppingCartAdminMetronic.Data.Components
{
	public class ProductsListViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public ProductsListViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<Product> products = await _context.Products.ToListAsync();
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
			ViewBag.Cart = cart;
			return View(products);
		}
	}
}
