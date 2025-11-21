# Cart Web API

A comprehensive cart management service built with .NET 9 that provides full cart functionality for e-commerce applications.

## Features

- **Add to Cart**: Add products to user's cart with automatic quantity management
- **Smart Quantity Management**: If same item is added, quantity is increased automatically  
- **Update Cart Items**: Modify quantity of existing cart items
- **Remove Items**: Remove specific items by cart item ID or user/product combination
- **Clear Cart**: Remove all items from a user's cart
- **Get Cart**: Retrieve all cart items for a user
- **Cart Summary**: Get quick overview with total items and quantities
- **Data Validation**: Comprehensive input validation and business rules
- **Error Handling**: Robust error handling with detailed responses

## Database Schema

The cart uses a SQL Server database with the following table structure:

```sql
CREATE TABLE [dbo].[CartItems] (
    [cart_item_id] int IDENTITY(1,1) PRIMARY KEY,
    [user_id] int NOT NULL,
    [product_id] int NOT NULL,
    [quantity] int NOT NULL DEFAULT 1,
    [created_date] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [updated_date] datetime2 NOT NULL DEFAULT GETUTCDATE()
);

-- Unique constraint to prevent duplicate user/product combinations
CREATE UNIQUE INDEX IX_CartItems_UserId_ProductId ON [dbo].[CartItems] ([user_id], [product_id]);
```

## API Endpoints

### Add to Cart
```http
POST /api/cart
Content-Type: application/json

{
  "userId": 1,
  "productId": 123,
  "quantity": 2
}
```

### Get User Cart
```http
GET /api/cart/user/{userId}
```

### Update Cart Item
```http
PUT /api/cart/{cartItemId}
Content-Type: application/json

{
  "quantity": 5
}
```

### Remove Cart Item (by ID)
```http
DELETE /api/cart/{cartItemId}
```

### Remove Cart Item (by User/Product)
```http
DELETE /api/cart/user/{userId}/product/{productId}
```

### Clear User Cart
```http
DELETE /api/cart/user/{userId}/clear
```

### Get Cart Summary
```http
GET /api/cart/user/{userId}/summary
```

## Business Rules

- **Maximum Quantity**: No more than 100 items per product
- **Minimum Quantity**: Must be at least 1
- **Unique Products**: Each user can have only one cart entry per product
- **Auto-Increment**: Adding existing product increases quantity
- **Input Validation**: All inputs are validated for type and business rules

## Response Formats

### Cart Item DTO
```json
{
  "cartItemId": 1,
  "userId": 1,
  "productId": 123,
  "quantity": 2,
  "createdDate": "2024-01-01T10:00:00Z",
  "updatedDate": "2024-01-01T10:30:00Z"
}
```

### Cart Summary DTO
```json
{
  "userId": 1,
  "items": [...],
  "totalItems": 3,
  "totalQuantity": 7
}
```

### Error Response
```json
{
  "error": "Validation failed: Quantity must be greater than 0"
}
```

## Docker Configuration

The service runs in Docker with:
- **Port**: 8080 (internal), 8009 (external)
- **Database**: SQL Server on separate container
- **Environment Variables**:
  - `DB_HOST`: Database server hostname
  - `DB_NAME`: Database name (cart)  
  - `DB_SA_PASSWORD`: Database password

## Architecture

- **Repository Pattern**: Clean separation of data access
- **Service Layer**: Business logic and validation
- **Dependency Injection**: All dependencies properly injected
- **Logging**: Comprehensive logging throughout the application
- **Validation Service**: Separate validation logic for extensibility
- **Error Handling**: Structured error handling with appropriate HTTP status codes

## Development

### Prerequisites
- .NET 9 SDK
- SQL Server (or Docker)
- Docker (for containerized deployment)

### Running Locally
```bash
dotnet restore
dotnet run
```

### Running with Docker
```bash
docker-compose up cartwebapi
```

The service will be available at `http://localhost:8009/api/cart`

## API Gateway Integration

All cart endpoints are accessible through the API Gateway at `http://localhost:8000/api/cart/*`

Note: When using the API Gateway, cart item ID parameters use `{cartItemId}` instead of `{cartId}`.

## Future Enhancements

- Integration with Customer service for user validation
- Integration with Product service for product validation
- Cart persistence across sessions
- Cart expiration policies
- Bulk operations (add multiple items at once)
- Cart sharing functionality
- Price calculation and totals