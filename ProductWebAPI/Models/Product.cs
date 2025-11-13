using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductWebAPI.Models
{
    [Table("product", Schema="dbo")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("product_id")]
        public int ProductId { get; set; }

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
        [Column("product_cover_image")]
        public string? ProductCoverImage { get; set; }
        [Column("product_gallery_images")]
        public List<string> ProductGalleryImages { get; set; }
    }
}
