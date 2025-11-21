using HandyCraftsAdapterWebAPI.Models;
using HandyCraftsAdapterWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HandyCraftsAdapterWebAPI.Controllers
{
    [Route("api/adapter")]
    [ApiController]
    public class HandyCraftsAdapter : ControllerBase
    {
        private readonly HandyCraftSyncService _service;
        private readonly ILogger<HandyCraftsAdapter> _logger;
        private readonly string _productsFilePath;
        private readonly string _paymentsFilePath;

        public HandyCraftsAdapter(HandyCraftSyncService service, ILogger<HandyCraftsAdapter> logger, IWebHostEnvironment env)
        {
            _service = service;
            _logger = logger;
            _productsFilePath = Path.Combine(env.ContentRootPath, "Data", "products.json");
            _paymentsFilePath = Path.Combine(env.ContentRootPath, "Data", "payments.json");
        }

        [HttpPost("sync/products")]
        public async Task<ActionResult<List<ProductCreateDto>>> SyncHandyCrafts()
        {
            var addedProducts = await _service.SyncAsync();
            return Ok(addedProducts);
        }

        /// <summary>
        /// Check availability for HandyCrafts products with quantity checking against JSON inventory
        /// </summary>
        /// <param name="request">Availability request with product ID and quantity</param>
        /// <returns>Availability status with latest pricing from JSON file</returns>
        [HttpPost("availability")]
        public async Task<ActionResult<AvailabilityResponseDto>> CheckAvailability([FromBody] AvailabilityRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ProductId))
                {
                    return BadRequest(new AvailabilityResponseDto
                    {
                        IsAvailable = false,
                        Message = "Product ID is required"
                    });
                }

                _logger.LogInformation("Checking availability for HandyCraft product {ProductId}", request.ProductId);
                
                // Load products from JSON file
                var products = await LoadProductsFromJsonAsync();
                var product = products.FirstOrDefault(p => 
                          p.code.Equals(request.ProductId, StringComparison.OrdinalIgnoreCase) && 
                     p.isActive);

                if (product == null)
                {
                    return Ok(new AvailabilityResponseDto
                    {
                        IsAvailable = false,
                        Message = $"Product '{request.ProductId}' not found or inactive"
                    });
                }

                // For HandyCrafts, quantity is most important
                var requestedQuantity = request.Quantity ?? 1;
                var availableStock = product.stockQuantity ?? 0;
                var isAvailable = availableStock >= requestedQuantity;

                await Task.Delay(100); // Simulate processing time

                return Ok(new AvailabilityResponseDto
                {
                    IsAvailable = isAvailable,
                    LatestPricePerUnit = product.price,
                    Currency = "LKR", // Sri Lankan Rupee for local handicrafts
                    AvailableQuantity = availableStock,
                    Message = isAvailable 
                    ? $"{availableStock} units available" 
                    : $"Insufficient stock. Only {availableStock} units available, but {requestedQuantity} requested"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for product {ProductId}", request?.ProductId);
                return StatusCode(500, new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = "An error occurred while checking availability"
                });
            }
        }

        /// <summary>
        /// Legacy GET endpoint for backward compatibility
        /// </summary>
        [HttpGet("availability/{productId}")]
        [Obsolete("Use POST /availability with AvailabilityRequestDto for enhanced functionality")]
        public async Task<ActionResult<bool>> CheckAvailability(string productId)
        {
            try
            {
                // Convert to new format and call the enhanced method
                var request = new AvailabilityRequestDto 
                { 
                    ProductId = productId,
                    Quantity = 1
                };
                
                var result = await CheckAvailability(request);
                if (result.Result is OkObjectResult okResult && okResult.Value is AvailabilityResponseDto response)
                {
                    return Ok(response.IsAvailable);
                }
                
                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for product {ProductId}", productId);
                return StatusCode(500, false);
            }
        }

        /// <summary>
        /// Process payment for HandyCrafts products with JSON file tracking and inventory update
        /// </summary>
        [HttpPost("payments/{productId}")]
        public async Task<ActionResult<bool>> ProcessPayment(string productId, [FromBody] PaymentProcessRequest? paymentRequest = null)
        {
            try
            {
                _logger.LogInformation("Processing payment for product {ProductId}", productId);

                // Load current products to get pricing and validate stock
                var products = await LoadProductsFromJsonAsync();
                var product = products.FirstOrDefault(p => 
                p.code.Equals(productId, StringComparison.OrdinalIgnoreCase) && 
            p.isActive);

                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for payment processing", productId);
                    return Ok(false);
                }

                var quantity = paymentRequest?.Quantity ?? 1;
                var availableStock = product.stockQuantity ?? 0;

                if (availableStock < quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Available}, Requested: {Requested}", 
                productId, availableStock, quantity);
                    return Ok(false);
                }

                // Calculate payment details
                var unitPrice = product.price;
                var totalAmount = unitPrice * quantity;
                var paymentId = Guid.NewGuid().ToString();

                // Create payment record
                var paymentRecord = new PaymentRecord
                {
                    PaymentId = paymentId,
                    ProductId = productId,
                    ProductCode = product.code,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalAmount = totalAmount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = "Completed", // Mock successful payment
                    CustomerInfo = paymentRequest?.CustomerInfo
                };

                // Save payment record to JSON
                await SavePaymentRecordAsync(paymentRecord);

                // Update product stock
                product.stockQuantity = availableStock - quantity;
                product.lastUpdated = DateTime.UtcNow;
                await UpdateProductsJsonAsync(products);

                _logger.LogInformation("Payment processed successfully. PaymentId: {PaymentId}, Product: {ProductId}, Quantity: {Quantity}, Total: {Total}", 
                paymentId, productId, quantity, totalAmount);

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for product {ProductId}", productId);
                return StatusCode(500, false);
            }
        }

        /// <summary>
        /// Get payment history for debugging and verification
        /// </summary>
        [HttpGet("payments/history")]
        public async Task<ActionResult<List<PaymentRecord>>> GetPaymentHistory()
        {
            try
            {
                if (!System.IO.File.Exists(_paymentsFilePath))
                {
                    return Ok(new List<PaymentRecord>());
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(_paymentsFilePath);
                var payments = JsonSerializer.Deserialize<List<PaymentRecord>>(jsonContent) ?? new List<PaymentRecord>();
    
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history");
                return StatusCode(500, "Error retrieving payment history");
            }
        }

        /// <summary>
        /// Get current product inventory from JSON file
        /// </summary>
        [HttpGet("products/inventory")]
        public async Task<ActionResult<List<object>>> GetProductInventory()
        {
            try
            {
                var products = await LoadProductsFromJsonAsync();
                var inventory = products.Where(p => p.isActive)
                    .Select(p => new {
                        productId = p.code,
                        name = p.name,
                        price = p.price,
                        stockQuantity = p.stockQuantity,
                        lastUpdated = p.lastUpdated
                    })
                    .ToList();
        
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product inventory");
                return StatusCode(500, "Error retrieving product inventory");
            }
        }

        /// <summary>
        /// Load products from JSON file
        /// </summary>
        private async Task<List<HandyCrafts>> LoadProductsFromJsonAsync()
        {
            try
            {
                if (!System.IO.File.Exists(_productsFilePath))
                {
                    _logger.LogWarning("Products JSON file not found at {FilePath}", _productsFilePath);
                    return new List<HandyCrafts>();
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(_productsFilePath);
                var products = JsonSerializer.Deserialize<List<HandyCrafts>>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return products ?? new List<HandyCrafts>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products from JSON file");
                return new List<HandyCrafts>();
            }
        }

        /// <summary>
        /// Update products JSON file with new stock levels
        /// </summary>
        private async Task UpdateProductsJsonAsync(List<HandyCrafts> products)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                        
                var jsonContent = JsonSerializer.Serialize(products, jsonOptions);
                await System.IO.File.WriteAllTextAsync(_productsFilePath, jsonContent);
            
                _logger.LogInformation("Products JSON file updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating products JSON file");
            }
        }

        /// <summary>
        /// Save payment record to JSON file
        /// </summary>
        private async Task SavePaymentRecordAsync(PaymentRecord paymentRecord)
        {
            try
            {
                List<PaymentRecord> payments;
                
                if (System.IO.File.Exists(_paymentsFilePath))
                {
                    var jsonContent = await System.IO.File.ReadAllTextAsync(_paymentsFilePath);
                    payments = JsonSerializer.Deserialize<List<PaymentRecord>>(jsonContent) ?? new List<PaymentRecord>();
                }
                else
                {
                    payments = new List<PaymentRecord>();
                }

                payments.Add(paymentRecord);

                var jsonOptions = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var updatedJsonContent = JsonSerializer.Serialize(payments, jsonOptions);
                await System.IO.File.WriteAllTextAsync(_paymentsFilePath, updatedJsonContent);
    
                _logger.LogInformation("Payment record saved successfully. PaymentId: {PaymentId}", paymentRecord.PaymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving payment record");
            }
        }
    }

    /// <summary>
    /// Request model for payment processing
    /// </summary>
    public class PaymentProcessRequest
    {
        public int Quantity { get; set; } = 1;
        public string? CustomerInfo { get; set; }
    }
}
