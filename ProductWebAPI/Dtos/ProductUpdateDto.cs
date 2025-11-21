using System.ComponentModel.DataAnnotations;

namespace ProductWebAPI.Dtos
{
    public class ProductUpdateDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string? ProductCoverImage { get; set; }
        public int? ProductQuantity { get; set; }
        public List<string> ProductGalleryImages { get; set; } = new();
        public Dictionary<string, object>? AdditionalAttributes { get; set; }
    }
}
