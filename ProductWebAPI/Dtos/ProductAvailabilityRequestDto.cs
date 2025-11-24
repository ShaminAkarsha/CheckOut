using System.ComponentModel.DataAnnotations;

namespace ProductWebAPI.Dtos
{
    public class ProductAvailabilityRequestDto
    {
        [Required]
        public int ProductId { get; set; } = 0;

        public string ExternalId { get; set; } = string.Empty;

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