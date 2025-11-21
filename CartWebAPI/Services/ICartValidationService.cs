using CartWebAPI.Models.DTOs;

namespace CartWebAPI.Services
{
  public interface ICartValidationService
    {
        Task<bool> ValidateUserExistsAsync(int userId);
        Task<bool> ValidateProductExistsAsync(int productId);
      ValidationResult ValidateAddToCartRequest(AddToCartDto request);
        ValidationResult ValidateUpdateCartRequest(UpdateCartItemDto request);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
  public List<string> Errors { get; set; } = new List<string>();
    }
}