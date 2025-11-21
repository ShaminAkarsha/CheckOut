using CartWebAPI.Models.DTOs;

namespace CartWebAPI.Services
{
    public class CartValidationService : ICartValidationService
    {
     private readonly ILogger<CartValidationService> _logger;
        // In a real application, you would inject HTTP clients or repositories 
        // to validate against Customer and Product services

     public CartValidationService(ILogger<CartValidationService> logger)
  {
   _logger = logger;
        }

 public async Task<bool> ValidateUserExistsAsync(int userId)
    {
  // In a real implementation, you would call the Customer service
            // For now, we'll assume any positive userId is valid
     await Task.CompletedTask;
            return userId > 0;
      }

    public async Task<bool> ValidateProductExistsAsync(int productId)
        {
     // In a real implementation, you would call the Product service
  // For now, we'll assume any positive productId is valid
     await Task.CompletedTask;
  return productId > 0;
   }

  public ValidationResult ValidateAddToCartRequest(AddToCartDto request)
      {
            var result = new ValidationResult { IsValid = true };

          if (request.UserId <= 0)
 {
       result.IsValid = false;
      result.Errors.Add("UserId must be greater than 0");
}

            if (request.ProductId <= 0)
   {
        result.IsValid = false;
    result.Errors.Add("ProductId must be greater than 0");
          }

      if (request.Quantity <= 0)
            {
        result.IsValid = false;
      result.Errors.Add("Quantity must be greater than 0");
     }

     if (request.Quantity > 100) // Business rule: max 100 items per product
     {
           result.IsValid = false;
           result.Errors.Add("Quantity cannot exceed 100 items");
            }

       return result;
        }

        public ValidationResult ValidateUpdateCartRequest(UpdateCartItemDto request)
        {
            var result = new ValidationResult { IsValid = true };

  if (request.Quantity <= 0)
      {
    result.IsValid = false;
 result.Errors.Add("Quantity must be greater than 0");
       }

       if (request.Quantity > 100) // Business rule: max 100 items per product
   {
         result.IsValid = false;
                result.Errors.Add("Quantity cannot exceed 100 items");
            }

            return result;
        }
    }
}