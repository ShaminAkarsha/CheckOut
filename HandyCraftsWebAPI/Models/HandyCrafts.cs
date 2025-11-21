namespace HandyCraftsAdapterWebAPI.Models
{
    public class HandyCrafts
    {
        public string name { get; set; }
        public string code { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string description { get; set; } = string.Empty;
        public string material { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string size { get; set; } = string.Empty;
        public string color { get; set; } = string.Empty;
        public string coverImage { get; set; } = string.Empty;
        public List<string> galleryImages { get; set; } = new();
        public string? weight { get; set; }
        public string? dimensions { get; set; }
        public string? artisan { get; set; }
        public string? origin { get; set; }
        public bool? isHandmade { get; set; }
        public string? craftingTechnique { get; set; }
        public int? stockQuantity { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdated { get; set; }
        public bool isActive { get; set; } = true;
    }
}
