using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingCartFrontMetronic.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartFrontMetronic.Models
{
	public class Product
	{
		public long Id { get; set; }

		[Required(ErrorMessage = "Please enter a value")]
		public string Name { get; set; }

		[BindNever]

		public string Slug { get; set; }

		[Required, MinLength(4, ErrorMessage = "Minimum length is 2")]
		public string Description { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Please enter a value")]
		[Column(TypeName = "decimal(8, 2)")]
		public decimal Price { get; set; }

		[Required, Range(1, int.MaxValue, ErrorMessage = "You must choose a category")]
		public long CategoryId { get; set; }

		public Category Category { get; set; }

		public string ThumbnailImage { get; set; } = "noimage.png";

		public List<ImageUrl> ImagesUrls { get; set; } = new List<ImageUrl>();

		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpload { get; set; }


		[NotMapped]
		[FileExtension]
		public List<IFormFile> ImageUploads { get; set; }

	}

}
