namespace ProductWebAPI.Dtos
{
    public class ProductCreateDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int? ProductQuantity { get; set; } = -1;
        public string ProductDescription { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string? ProductCoverImage { get; set; }
        public List<string> ProductGalleryImages { get; set; } = new();
        public Dictionary<string, object>? AdditionalAttributes { get; set; }
    }
}
