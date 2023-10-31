using System.ComponentModel.DataAnnotations;

namespace ShoppingCartFrontMetronic.Models
{
	public class Admin
	{
		public string Id { get; set; }

		[Required, MinLength(2, ErrorMessage = "Minimum UserName length is 2")]
		[Display(Name = "Username")]
		public string UserName { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minimum Password length is 4")]
		public string Password { get; set; }
	}

}
