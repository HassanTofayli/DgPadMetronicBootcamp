using Microsoft.AspNetCore.Identity;

namespace ShoppingCartFrontMetronic.Models
{
	public class AppUser : IdentityUser
	{
		public string Occupation { get; set; } = "";
	}
}
