using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesWebApi.Models
{
    [Table("Clothes", Schema = "dbo")]
    public class Clothes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Cloth_id")]
        public int ClothId { get; set; }

        [Column("Cloth_title")]
        public string ClothTitle { get; set; }

        [Column("brand")]
        public string Brand { get; set; }

        [Column("size")]
        public string Size { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("Cloth_description")]
        public string ClothDescription { get; set; }

        [Column("Cloth_category")]
        public string ClothCategory { get; set; }

        [Column("available_units")]
        public int ClothStock { get; set; }

        [Column("Cloth_cover_image")]
        public int ClothCount { get; set; }

        [Column("cloth_gallery_images")]
        public List<string> ClothGalleryImages { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime LastUpdatedDate { get; set; }
    }
}
