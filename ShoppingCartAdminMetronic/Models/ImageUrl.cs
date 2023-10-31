namespace ShoppingCartAdminMetronic.Models
{
	public class ImageUrl
	{

		public long Id { get; set; }
		public string url { get; set; }
		public virtual Product product { get; set; }
		public long productId { get; set; }

	}
}
