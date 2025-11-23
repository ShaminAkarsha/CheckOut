namespace OrderWebAPI.Dtos

{
    public class ProductAvailabilityResponseDto
    {
        /// <summary>
        /// Indicates if the product is available
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Product ID from the database
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; } = string.Empty;

        /// <summary>
        /// Product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Current price per unit
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Available quantity in stock
        /// </summary>
        public int? AvailableQuantity { get; set; }

        /// <summary>
        /// Requested quantity for availability check
        /// </summary>
        public int RequestedQuantity { get; set; }

        /// <summary>
        /// Additional message about availability
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Source system of the product
        /// </summary>
        public string Source { get; set; } = string.Empty;
    }
}