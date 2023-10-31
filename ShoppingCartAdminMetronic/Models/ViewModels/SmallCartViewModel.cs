namespace ShoppingCartAdminMetronic.Models.ViewModels
{
	public class SmallCartViewModel
	{
		public int NumberOfItems { get; set; }
		public decimal TotalAmount { get; set; }
		public List<CartItem> CartItems { get; set; } = new List<CartItem>();
	}
}