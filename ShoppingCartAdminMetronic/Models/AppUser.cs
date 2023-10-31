using Microsoft.AspNetCore.Identity;

namespace ShoppingCartAdminMetronic.Models
{
	public class AppUser : IdentityUser
	{
		public string Occupation { get; set; } = "";
	}
}
