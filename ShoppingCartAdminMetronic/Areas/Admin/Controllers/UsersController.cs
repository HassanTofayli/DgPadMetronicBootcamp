using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartAdminMetronic.Models;
using ShoppingCartAdminMetronic.Models.ViewModels;

namespace ShoppingCartAdminMetronic.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin, Manager")]
	public class UsersController : Controller
	{
		private UserManager<AppUser> _userManager;
		private RoleManager<IdentityRole> _roleManager;

		public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}


		public IActionResult Index()
		{

			return View(_userManager.Users.ToList());
		}

		public IActionResult Create()
		{
			ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem { Text = i, Value = i });
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Models.Admin admin, string role)
		{
			if (ModelState.IsValid)
			{
				AppUser newUser = new AppUser { UserName = admin.UserName, Email = admin.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, admin.Password);

				if (result.Succeeded)
				{
					if (role != "norole")
						await _userManager.AddToRoleAsync(newUser, role);
					return RedirectToAction("Index");
				}

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			ViewBag.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem { Text = i, Value = i });

			return View(admin);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			AppUser user = await _userManager.FindByIdAsync(id);
			UserViewModel userEdit = new(user);
			return View(userEdit);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(UserViewModel user)
		{
			if (ModelState.IsValid)
			{
				AppUser appUser = await _userManager.FindByIdAsync(user.Id);
				appUser.UserName = user.UserName;
				appUser.Email = user.Email;

				IdentityResult result = await _userManager.UpdateAsync(appUser);

				if (result.Succeeded && !String.IsNullOrEmpty(user.Password))
				{
					await _userManager.RemovePasswordAsync(appUser);
					result = await _userManager.AddPasswordAsync(appUser, user.Password);
				}

				if (result.Succeeded) return RedirectToAction("Index");

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(user);
		}
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			AppUser user = await _userManager.FindByIdAsync(id);


			if (user != null) await _userManager.DeleteAsync(user);

			return RedirectToAction("Index");
		}

	}
}
