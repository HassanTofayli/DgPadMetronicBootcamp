using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAdminMetronic.Data;
using ShoppingCartAdminMetronic.Models;

namespace ShoppingCartAdminMetronic.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public CategoriesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;

		}

		public async Task<IActionResult> Index()
		{
			var categories = await _context.Categories.ToListAsync();
			return View(categories);
		}

		[Authorize(Roles = "Admin, Manager")]
		[Route("Categories/Create")]

		public IActionResult Create()
		{
			return View();
		}
		[Authorize(Roles = "Admin, Manager")]
		[Route("Categories/Create")]

		[HttpPost]
		public async Task<IActionResult> Create(Category category)
		{

			category.Slug = category.Name.ToLower().Replace(" ", "-");

			if (ModelState.IsValid)
			{
				if (category.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/CategoriesThumbnail");
					string imageName = Guid.NewGuid().ToString() + "_" + category.ImageUpload.FileName;

					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await category.ImageUpload.CopyToAsync(fs);
					fs.Close();

					category.ThumbnailImage = imageName;
				}
			}

			_context.Add(category);
			await _context.SaveChangesAsync();

			TempData["Success"] = "The product has been created!";

			return RedirectToAction("Index");
		}



		[Authorize(Roles = "Admin, Manager")]

		public async Task<IActionResult> List()
		{
			return View(await _context.Categories
						  .ToListAsync());
		}
		[Authorize(Roles = "Admin, Manager")]

		[HttpPost]
		public async Task<IActionResult> Delete(long id)
		{

			Category c = await _context.Categories.FindAsync(id);
			if (c == null) { return new ContentResult { Content = "There was an error", ContentType = "text/plain" }; }


			string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/CategoriesThumbnail", c.ThumbnailImage);

			if (System.IO.File.Exists(imagePath))
			{
				System.IO.File.Delete(imagePath);
			}

			_context.Categories.Remove(c);
			await _context.SaveChangesAsync();

			TempData["Success"] = "The Category has been Deleted!";
			return RedirectToAction("Index");
		}



		[Route("Category/{name}")]

		public async Task<IActionResult> CategoryType(string name)
		{
			var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

			if (category == null)
			{
				var product = await _context.Products.ToListAsync();
				ViewBag.Categories = await _context.Categories.ToListAsync();

				return View(product);
			}

			var products = await _context.Products.Where(p => p.CategoryId == category.Id).ToListAsync();
			ViewBag.Categories = await _context.Categories.ToListAsync();
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
			ViewBag.Cart = cart;
			return View(products);
		}


	}
}