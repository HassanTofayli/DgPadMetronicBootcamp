using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAdminMetronic.Models;

namespace ShoppingCartAdminMetronic.Data.Components
{
	public class SidebarCategoriesListViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public SidebarCategoriesListViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<Category> categories = await _context.Categories.ToListAsync();

			return View(categories);
		}
	}
}
