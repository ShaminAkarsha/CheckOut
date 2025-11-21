using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderWebAPI.Models
{
    [Table("Order", Schema = "dbo")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("status")]
        // Pending, Confirmed, Failed, Cancelled, Expired, etc.
        public string Status { get; set; } = "Pending";

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("currency")]
        public string Currency { get; set; } = "USD";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<OrderItem> Items { get; set; } = new();
        public OrderPayment? Payment { get; set; }
    }
}
