namespace ProductWebAPI.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public string? ProductCoverImage { get; set; }
        public List<string> ProductGalleryImages { get; set; }
    }
}
