using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAdminMetronic.Models;
using ShoppingCartAdminMetronic.Models.ViewModels;

namespace ShoppingCartAdminMetronic.Controllers
{
	public class AccountController : Controller
	{

		private SignInManager<AppUser> _signInManager;
		private UserManager<AppUser> _userManager;

		public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[Route("Login")]
		public IActionResult Login(string returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

		[HttpPost]
		[Route("Login")]

		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);

				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}

				ModelState.AddModelError("", "Invalid username or password");
			}
			return View(loginVM);
		}


		[Route("/SignUp")]
		public IActionResult Signup() => View();

		[HttpPost]
		[Route("/SignUp")]

		public async Task<IActionResult> Signup(Admin admin)
		{
			if (ModelState.IsValid)
			{
				AppUser newUser = new AppUser { UserName = admin.UserName, Email = admin.Email };
				IdentityResult result = await _userManager.CreateAsync(newUser, admin.Password);

				if (result.Succeeded)
				{

					Microsoft.AspNetCore.Identity.SignInResult resultin = await _signInManager.PasswordSignInAsync(admin.UserName, admin.Password, false, false);

					if (resultin.Succeeded)
					{
						return Redirect("/");
					}

					ModelState.AddModelError("", "Invalid username or password");
					return BadRequest();

				}

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(admin);
		}

		//public IActionResult Details() => View(new AuthDetailsViewModel { Cookie = Request.Cookies[".AspNetCore.Identity.Application"] });
		public async Task<IActionResult> Details()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

				return View(new AuthDetailsViewModel { Cookie = Request.Cookies[".AspNetCore.Identity.Application"], User = user });
			}

			return View(new AuthDetailsViewModel());
		}

		//public async Task Logout() => await _signInManager.SignOutAsync();
		public async Task<RedirectResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();

			return Redirect(returnUrl);
		}

		[Authorize]
		public string AllRoles() => "All Roles";
		[Authorize(Roles = "Admin")]
		public string AdminRoles() => "admin Roles";
		[Authorize(Roles = "Manager")]
		public string ManagerRoles() => "Manager Roles";
		//[Route("AccessDenied")]
		[Route("AccessDenied")]
		public string AccessDenied() => "AccessDenied";


	}
}
