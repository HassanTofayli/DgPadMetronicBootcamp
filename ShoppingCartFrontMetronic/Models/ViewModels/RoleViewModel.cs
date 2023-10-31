using Microsoft.AspNetCore.Identity;

namespace ShoppingCartFrontMetronic.Models.ViewModels
{
	public class RoleViewModel
	{
		public IdentityRole Role { get; set; }
		public IEnumerable<AppUser> Members { get; set; }
		public IEnumerable<AppUser> NonMembers { get; set; }

		public string RoleName { get; set; }
		public string[] AddIds { get; set; }
		public string[] DeleteIds { get; set; }

	}
}
