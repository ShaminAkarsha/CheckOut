using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderWebAPI.Models
{
    [Table("OrderPayment", Schema = "dbo")]
    public class OrderPayment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [ForeignKey("Order")]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("payment_status")]
        // pending, success, failed
        public string PaymentStatus { get; set; } = "pending";

        [Column("payment_reference")]
        // Payment token or provider transaction ID
        public string? PaymentReference { get; set; }

        [Column("payment_url")]
        // If external provider returns redirect URL
        public string? PaymentUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
    }
}
