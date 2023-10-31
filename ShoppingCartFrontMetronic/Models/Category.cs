using ShoppingCartFrontMetronic.Data.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartFrontMetronic.Models
{
	public class Category
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string ThumbnailImage { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpload { get; set; }
	}
}
