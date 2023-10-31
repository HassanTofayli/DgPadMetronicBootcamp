using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAdminMetronic.Data;
using ShoppingCartAdminMetronic.Models;

namespace ShoppingCartAdminMetronic.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin, Manager")]

	public class ProductsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{

			return View(await _context.Products
									  .Include(p => p.Category)
									  .Include(p => p.ImagesUrls)
									  .ToListAsync());

		}

		public IActionResult AddProduct()
		{
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddProduct(Product product)
		{
			product.Slug = product.Name.ToLower().Replace(" ", "-");
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

			if (ModelState.IsValid)
			{

				var slug = await _context.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "The product already exists.");
					return View(product);
				}

				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();

					product.ThumbnailImage = imageName;
				}
				if (product.ImageUploads != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					foreach (var upload in product.ImageUploads)
					{
						ImageUrl url = new ImageUrl();
						string imageName = Guid.NewGuid().ToString() + "_" + upload.FileName;
						string filePath = Path.Combine(uploadsDir, imageName);

						FileStream fs = new FileStream(filePath, FileMode.Create);
						await upload.CopyToAsync(fs);

						url.product = product;
						url.productId = product.Id;
						url.url = imageName;
						product.ImagesUrls.Add(url);
						fs.Close();
					}
				}

				_context.Add(product);
				await _context.SaveChangesAsync();

				TempData["Success"] = "The product has been created!";

				return RedirectToAction("Index");
			}

			return View(product);
		}



		public async Task<IActionResult> EditProduct(long id)
		{
			Product p = await _context.Products.FindAsync(id);
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", p.CategoryId);
			List<ImageUrl> images = await _context.ImageUrls.Where(i => i.productId == id).ToListAsync();
			if (images != null || images.Count > 0)
				ViewBag.ImagesList = images;
			p.ImagesUrls = images;
			return View(p);
		}

		[HttpPost]
		public async Task<IActionResult> EditProduct(long id, Product product, string DeletedImages)
		{
			var deletedImagesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImageUrl>>(DeletedImages);

			Product existingProduct = await _context.Products.FindAsync(id);

			string newSlug = product.Name.ToLower().Replace(" ", "-");

			// Check for duplicate slugs
			var duplicateSlug = await _context.Products
									.Where(p => p.Id != id) // Exclude the current product
									.FirstOrDefaultAsync(p => p.Slug == newSlug);

			if (duplicateSlug != null)
			{
				ModelState.AddModelError("Name", "The product name produces a URL slug that is already in use.");
			}
			ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

			if (ModelState.IsValid)
			{
				if (product.ImageUpload != null)
				{
					if (existingProduct.ThumbnailImage != "noimage.png")
					{
						string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/products", existingProduct.ThumbnailImage);
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string newImageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string newImagePath = Path.Combine(uploadsDir, newImageName);

					FileStream fs = new FileStream(newImagePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();

					existingProduct.ThumbnailImage = newImageName;
				}

				if (product.ImageUploads != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					foreach (var upload in product.ImageUploads)
					{
						ImageUrl url = new ImageUrl();
						string imageName = Guid.NewGuid().ToString() + "_" + upload.FileName;
						string filePath = Path.Combine(uploadsDir, imageName);

						FileStream fs = new FileStream(filePath, FileMode.Create);
						await upload.CopyToAsync(fs);

						url.product = product;
						url.productId = product.Id;
						url.url = imageName;
						product.ImagesUrls.Add(url);
						fs.Close();
						_context.ImageUrls.Add(new ImageUrl { url = imageName, productId = product.Id });
					}
				}

				if (deletedImagesList.Count > 0)
				{
					_context.ImageUrls.RemoveRange(deletedImagesList);

				}




				existingProduct.Name = product.Name;
				existingProduct.Description = product.Description;
				existingProduct.Price = product.Price;
				existingProduct.CategoryId = product.CategoryId;
				existingProduct.Slug = product.Slug;

				_context.Update(existingProduct);
				await _context.SaveChangesAsync();

				TempData["Success"] = "The product has been Updated!";
			}
			List<ImageUrl> images = await _context.ImageUrls.Where(i => i.productId == id).ToListAsync();
			if (images != null || images.Count > 0)
				ViewBag.ImagesList = images;
			existingProduct.ImagesUrls = images;
			return RedirectToAction("Index");
		}



		public async Task<IActionResult> DeleteProduct(long id)
		{
			Product p = await _context.Products.FindAsync(id);
			if (p == null) { return new ContentResult { Content = "There was an error", ContentType = "text/plain" }; }

			if (!string.Equals(p.ThumbnailImage, "noimage.png"))
			{
				string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/products", p.ThumbnailImage);

				if (System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}
			}

			_context.Products.Remove(p);
			await _context.SaveChangesAsync();

			TempData["Success"] = "The product has been Deleted!";
			return RedirectToAction("Index");
		}




		// NOW THE CODE OF THE AJAX
		[HttpPost]
		public async Task<ActionResult> ChangeName(long id, string name)
		{
			Console.Write("____________________________________________________________________________________________________");
			Console.Write("I am In change name controller");
			Console.WriteLine($"Received id: {id}, name: {name}");
			try
			{
				Console.WriteLine("I am in try");
				Product p = await _context.Products.FindAsync(id);
				Console.WriteLine("I am after findAsync");

				if (p == null)
				{
					Console.WriteLine("Product not found");
					return Json(new { success = false, message = "Product not found" });
				}

				p.Name = name;
				_context.Products.Update(p);
				await _context.SaveChangesAsync();

				Console.WriteLine("Name updated successfully");
				return Json(new { success = true, message = "Name updated successfully" });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
				return Json(new { success = false, message = "An error occurred while updating the name" });
			}
		}
		[HttpPost]
		public async Task<ActionResult> ChangePrice(long id, decimal price)
		{
			Console.Write("____________________________________________________________________________________________________");
			Console.Write("I am In change price controller");
			Console.WriteLine($"Received id: {id}, price: {price}");
			try
			{
				Console.WriteLine("I am in try");
				Product p = await _context.Products.FindAsync(id);
				Console.WriteLine("I am after findAsync");

				if (p == null)
				{
					Console.WriteLine("Product not found");
					return Json(new { success = false, message = "Product not found" });
				}

				p.Price = price;
				_context.Products.Update(p);
				await _context.SaveChangesAsync();

				Console.WriteLine("Price updated successfully");
				return Json(new { success = true, message = "Price updated successfully" });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
				return Json(new { success = false, message = "An error occurred while updating the price" });
			}
		}


		public async Task<ActionResult> DeleteAjax(long id)
		{
			try
			{
				Console.WriteLine("I am in try");
				Product p = await _context.Products.FindAsync(id);
				Console.WriteLine("I am after findAsync");

				if (p == null)
				{
					Console.WriteLine("Product not found");
					return Json(new { success = false, message = "Product not found" });
				}

				_context.Products.Remove(p);
				await _context.SaveChangesAsync();

				Console.WriteLine("Price Deleted successfully");
				return Json(new { success = true, message = "Price Deleted successfully" });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
				return Json(new { success = false, message = "An error occurred while Deleting the price" });
			}
		}

	}
}


