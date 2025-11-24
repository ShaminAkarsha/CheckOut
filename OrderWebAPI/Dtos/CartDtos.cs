namespace OrderWebAPI.Dtos
{
    public class CartItemDto
    {
      public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class CartSummaryDto
    {
        public int UserId { get; set; }
       public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
     public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
    }
}