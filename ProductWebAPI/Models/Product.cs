using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProductWebAPI.Models
{
    [Table("product", Schema = "dbo")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("external_id")]
        public string ExternalId { get; set; }

        [Column("source")]
        public string Source { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("product_price")]
        public decimal ProductPrice { get; set; }

        [Column("product_description")]
        public string ProductDescription { get; set; }

        [Column("product_category")]
        public string ProductCategory { get; set; }

        [Column("product_quantity")]
        public int? ProductQuantity { get; set; }

        [Column("product_cover_image")]
        public string? ProductCoverImage { get; set; }

        [Column("product_gallery_images")]
        public string ProductGalleryImagesJson { get; set; }

        [NotMapped]
        public List<string> ProductGalleryImages
        {
            get => string.IsNullOrEmpty(ProductGalleryImagesJson)
                ? new()
                : JsonSerializer.Deserialize<List<string>>(ProductGalleryImagesJson);
            set => ProductGalleryImagesJson = JsonSerializer.Serialize(value);
        }

        [Column("additional_attributes")]
        public string? AdditionalAttributesJson { get; set; }

        [NotMapped]
        public Dictionary<string, object>? AdditionalAttributes
        {
            get => string.IsNullOrEmpty(AdditionalAttributesJson)
                ? null
                : JsonSerializer.Deserialize<Dictionary<string, object>>(AdditionalAttributesJson);
            set => AdditionalAttributesJson = value == null
                ? null
                : JsonSerializer.Serialize(value);
        }

        [Column("synced_at")]
        public DateTime SyncedAt { get; set; }
    }
}
