using Microsoft.AspNetCore.Mvc;
using ShoppingCartFrontMetronic.Data;
using ShoppingCartFrontMetronic.Models;
using ShoppingCartFrontMetronic.Models.ViewModels;

namespace ShoppingCartFrontMetronic.Controllers
{
	public class CartController : Controller
	{
		private readonly AppDbContext _context;

		public CartController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			CartViewModel cartVM = new()
			{
				CartItems = cart,
				GrandTotal = cart.Sum(x => x.Quantity * x.Price)
			};

			return View(cartVM);
		}

		public async Task<IActionResult> Add(long id)
		{
			Product product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return Json(new { success = false, message = "Product not found" });
			}

			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem == null)
			{
				cartItem = new CartItem(product);
				cart.Add(cartItem);
			}
			else
			{
				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);
			var sum = cart?.Sum(x => x.Quantity * x.Price) ?? 0;
			ViewBag.sum = sum;

			return Json(new
			{
				success = true,
				message = "The product has been added",
				cartItem,
				cartSum = sum.ToString("C2")
			});
		}

		public async Task<IActionResult> Increase(long id)
		{


			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem != null)
			{
				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);
			var sum = cart?.Sum(x => x.Quantity * x.Price) ?? 0;
			ViewBag.sum = sum;

			return Json(new
			{
				success = true,
				message = "The product has been increased",
				cartItem,
				cartSum = sum.ToString("C2")
			});
		}

		public IActionResult Decrease(long id)
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

			CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem.Quantity > 1)
			{
				cartItem.Quantity -= 1;
			}
			else
			{
				RedirectToAction("Remove", new { id = id });
			}

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			var sum = cart?.Sum(x => x.Quantity * x.Price) ?? 0;
			ViewBag.sum = sum;

			return Json(new
			{
				success = true,
				message = "The product has been decreased",
				cartItem,
				cartSum = sum.ToString("C2")
			});
		}

		public async Task<IActionResult> Remove(long id)
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			bool success = false;

			CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			var nbOfRemoved = cart.RemoveAll(c => c.ProductId == id);


			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
			if (nbOfRemoved > 0)
				success = true;

			var sum = cart?.Sum(x => x.Quantity * x.Price) ?? 0;
			ViewBag.sum = sum;

			return Json(new
			{
				success,
				remove = true,
				cartItem,
				cartSum = sum.ToString("C2")
			});
		}
		public async Task<IActionResult> Clear(long id)
		{
			HttpContext.Session.Remove("Cart");
			ViewBag.sum = 0;

			return RedirectToAction("Index");
		}
	}
}