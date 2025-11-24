using System.ComponentModel.DataAnnotations;

namespace OrderWebAPI.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }
      
        /// <summary>
 /// Optional payment information for immediate processing
     /// </summary>
    public string? PaymentMethod { get; set; }
        
        /// <summary>
        /// Optional delivery/booking information
        /// </summary>
        public DateTime? PreferredDeliveryDate { get; set; }
   
        /// <summary>
    /// Additional order notes
      /// </summary>
        public string? Notes { get; set; }
    }

    public class OrderDto
    {
      public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public OrderPaymentDto? Payment { get; set; }
    }

    public class OrderItemDto
  {
     public int OrderItemId { get; set; }
public int ProductId { get; set; }
      public string ProductName { get; set; } = string.Empty;
    public int AdapterId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
      public string? ProviderReservationId { get; set; }
        public string ProviderStatus { get; set; } = string.Empty;
    }

    public class OrderPaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderSummaryDto
    {
    public int OrderId { get; set; }
     public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
      public List<string> ProductNames { get; set; } = new();
    }
}