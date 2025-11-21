using System.ComponentModel.DataAnnotations;

namespace HandyCraftsAdapterWebAPI.Models
{
    public class AvailabilityRequestDto
    {
        [Required]
      public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantity for inventory-based products
 /// </summary>
 public int? Quantity { get; set; }

        /// <summary>
    /// Check-in date for booking-based products
        /// </summary>
  public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// Check-out date for booking-based products
        /// </summary>
   public DateTime? CheckOutDate { get; set; }

/// <summary>
        /// Number of guests for booking-based products
        /// </summary>
        public int? NumberOfGuests { get; set; }
    }
}