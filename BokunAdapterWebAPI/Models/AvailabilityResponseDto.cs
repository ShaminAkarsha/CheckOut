namespace BokunAdapterWebAPI.Models
{
    public class AvailabilityResponseDto
    {
 /// <summary>
     /// Indicates if the product/service is available
        /// </summary>
 public bool IsAvailable { get; set; }

        /// <summary>
    /// Latest price per unit (per night for hotels, per person for tours, per item for products)
        /// </summary>
  public decimal? LatestPricePerUnit { get; set; }

        /// <summary>
       /// Currency code for the price (e.g., USD, EUR, GBP)
        /// </summary>
    public string? Currency { get; set; }

     /// <summary>
        /// Available quantity (for inventory-based products)
        /// </summary>
        public int? AvailableQuantity { get; set; }

        /// <summary>
    /// Optional message from the adapter (e.g., "Limited availability", "Fully booked")
        /// </summary>
 public string? Message { get; set; }
    }
}