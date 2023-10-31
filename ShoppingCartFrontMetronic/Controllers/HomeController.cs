using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartFrontMetronic.Data;
using ShoppingCartFrontMetronic.Models;
using System.Diagnostics;

namespace ShoppingCartFrontMetronic.Controllers
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		public HomeController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{

			return View();

		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[Route("/Search")]
		public async Task<IActionResult> Search(string search)
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
			ViewBag.Cart = cart;
			ViewBag.search = search;

			List<string> recentlySearched = HttpContext.Session.GetJson<List<string>>("recentlySearched") ?? new List<string>();


			if (search != null && !recentlySearched.Contains(search))
			{
				recentlySearched.Add(search);
				HttpContext.Session.SetJson("recentlySearched", recentlySearched);
			}

			List<Product> items = await _context.Products.Where(pr => pr.Name.Contains(search)).ToListAsync();
			return View(items);
		}

		public IActionResult ClearSearch()
		{
			@HttpContext.Session.Remove("recentlySearched");
			return Ok();
		}
	}
}