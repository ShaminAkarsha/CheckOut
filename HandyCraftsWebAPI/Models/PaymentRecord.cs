namespace HandyCraftsAdapterWebAPI.Models
{
 public class PaymentRecord
    {
  public string PaymentId { get; set; } = string.Empty;
  public string ProductId { get; set; } = string.Empty;
 public string ProductCode { get; set; } = string.Empty;
 public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
     public DateTime PaymentDate { get; set; }
 public string PaymentStatus { get; set; } = string.Empty; // "Completed", "Failed", "Pending"
    public string? CustomerInfo { get; set; }
    }
}