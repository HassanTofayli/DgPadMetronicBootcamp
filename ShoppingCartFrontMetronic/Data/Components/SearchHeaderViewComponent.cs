using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartFrontMetronic.Data.Components
{
	public class SearchHeaderViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public SearchHeaderViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<string> recentlySearched = HttpContext.Session.GetJson<List<string>>("recentlySearched") ?? new List<string>();
			if (recentlySearched.Count() > 0)
				ViewBag.RecentlySearched = recentlySearched;
			return View();
		}
	}
}
