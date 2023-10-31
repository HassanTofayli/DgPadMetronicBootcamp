using Microsoft.AspNetCore.Mvc;
using ShoppingCartAdminMetronic.Models;
using ShoppingCartAdminMetronic.Models.ViewModels;

namespace ShoppingCartAdminMetronic.Data.Components
{
	public class SmallCartViewComponent : ViewComponent
	{
		private readonly AppDbContext _context;

		public SmallCartViewComponent(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			SmallCartViewModel smallCartVM;

			if (cart == null || cart.Count == 0)
			{
				smallCartVM = new()
				{
					NumberOfItems = 0,
					TotalAmount = 0,
					CartItems = new List<CartItem> { }
				};
			}
			else
			{
				smallCartVM = new()
				{
					NumberOfItems = cart.Sum(x => x.Quantity),
					TotalAmount = cart.Sum(x => x.Quantity * x.Price)
				};
				foreach (var item in cart) { smallCartVM.CartItems.Add(new CartItem { ProductName = item.ProductName, Price = item.Price, Image = item.Image, Quantity = item.Quantity, ProductId = item.ProductId }); }
			}
			if (cart != null)
				Console.WriteLine(cart.First().Quantity.ToString());
			ViewBag.sum = cart?.Sum(x => x.Quantity * x.Price) ?? 0;

			return View(smallCartVM);
		}
	}
}
