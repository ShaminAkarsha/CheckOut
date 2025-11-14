using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetsWebApi.Models
{
    [Table("Sweets", Schema = "dbo")]
    public class Sweets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Sweet_id")]
        public int SweetId { get; set; }
        
        [Column("Sweet_name")]
        public string SweetTitle { get; set; }
        
        [Column("manufacture")]
        public string Manufacturer { get; set; }

        [Column("sweet_flavor")]
        public string SweetFlavor { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("Sweet_description")]
        public string SweetDescription { get; set; }

        [Column("Sweet_category")]
        public string SweetCategory { get; set; }

        [Column("Suger_level")]
        public string SugerLevel { get; set; }

        [Column("available_units")]
        public int SweetStock { get; set; }

        [Column("Sweet_cover_image")]
        public string? SweetCoverImage { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTime LastUpdatedDate { get; set; }

    }
}
