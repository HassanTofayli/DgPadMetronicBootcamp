using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartFrontMetronic.Models;

namespace ShoppingCartFrontMetronic.Data.Components
{
	public class CategoriesListViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public CategoriesListViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<Category> categories = await _context.Categories.ToListAsync();
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
			ViewBag.Cart = cart;
			return View(categories);
		}
	}
}
