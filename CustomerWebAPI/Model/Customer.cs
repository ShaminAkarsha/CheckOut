using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerWebAPI.Model
{
    [Table("customer", Schema ="dbo")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("customer_id")]  
        public int CustomerId {  get; set; }
        [Column("customer_name")]
        public string CustomerName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("mobile_number")]
        public string MobileNumber { get; set; }

    }
}
