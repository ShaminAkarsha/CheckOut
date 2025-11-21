using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderWebAPI.Models
{
    [Table("OrderItem", Schema = "dbo")]
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }

        [ForeignKey("Order")]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }  // from ProductService

        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("adapter_id")]
        public int AdapterId { get; set; } // which adapter will process it

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("provider_reservation_id")]
        // Returned by the external provider after availability check
        public string? ProviderReservationId { get; set; }

        [Column("provider_status")]
        // pending, reserved, confirmed, failed
        public string ProviderStatus { get; set; } = "pending";

        public Order? Order { get; set; }
    }
}
