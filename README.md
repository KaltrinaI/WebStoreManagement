
# WebStoreApp

WebStoreApp is a full-fledged e-commerce platform that allows users to browse products, place orders, and manage their accounts. Admins can manage product details, users, orders, and discounts.

## Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8 SDK**: [Download from the official .NET website](https://dotnet.microsoft.com/download/dotnet/8.0).
- **PostgreSQL**: [Download from the official PostgreSQL website](https://www.postgresql.org/download/).

## Getting Started

To get started, follow these steps:

### Clone the Repository

```bash
git clone https://github.com/yourusername/WebStoreApp.git
cd WebStoreApp
```

### Install Dependencies

1. **Backend (API)**:
   - Navigate to the WebStoreApp directory.
   - Run the following to install backend dependencies:
   ```bash
   dotnet restore
   ```


### Database Setup

1. **Create a Database**: Create a PostgreSQL database named `WebStoreManagement`.

2. **Configure Connection String**: In the `appsettings.json` file, set up the connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=WebStoreManagement;Username=yourusername;Password=yourpassword"
     }
   }
   ```

### Installing Required Packages

Install the necessary NuGet packages:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.10
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package AutoMapper
```

These packages are essential for PostgreSQL integration, identity management, and object mapping.

### Applying Migrations

1. **Add Migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. **Update Database**:
   ```bash
   dotnet ef database update
   ```

### Running the Application

1. **Backend**:
   - To run the backend API:
   ```bash
   dotnet run
   ```

## Available Endpoints

#AuthController

## POST /api/auth/register
Registers a new user.

**Request Body**: 
- FirstName, LastName, Email, Password, PhoneNumber

## POST /api/auth/login
Authenticates a user and returns a JWT token.

**Request Body**: 
- Email, Password


## POST /api/auth/role
Creates a new role.

**Request Body**: 
- roleName

## POST /api/auth/assign
Assigns a role to a user.

**Request Body**: 
- username, roleName

#BrandController

## GET /api/brand/id/{BrandId}
Retrieves a brand by ID.

## GET /api/brand/name/{BrandName}
Retrieves a brand by name.

## GET /api/brand
Retrieves all brands.

## POST /api/brand
Adds a new brand.

**Request Body**: 
- BrandName (string)

## PUT /api/brand/{BrandId}
Updates an existing brand.

**Request Body**: 
- BrandName (string)

## DELETE /api/brand/{BrandId}
Deletes a brand.

#CategoryController

## GET /api/category/id/{CategoryId}
Retrieves a category by ID.


## GET /api/category/name/{CategoryName}
Retrieves a category by name.


## GET /api/category
Retrieves all categories.


## POST /api/category
Adds a new category.

**Request Body**: 
- CategoryName (string)


## PUT /api/category/{CategoryId}
Updates an existing category.

**Request Body**: 
- CategoryName (string)

## DELETE /api/category/{CategoryId}
Deletes a category.

#ColorController

## GET /api/color/id/{ColorId}
Retrieves a color by ID.

## GET /api/color/name/{ColorName}
Retrieves a color by name.

## GET /api/color
Retrieves all colors.

## POST /api/color
Adds a new color.

**Request Body**: 
- ColorName (string)

## PUT /api/color/{ColorId}
Updates an existing color.

**Request Body**: 
- ColorName (string)

## DELETE /api/color/{ColorId}
Deletes a color.

#DiscountController

## GET /api/discount/{discountId}
Retrieves a discount by ID.

## GET /api/discount
Retrieves all discounts.

## GET /api/discount/by-name/{name}
Retrieves discounts by name.

## GET /api/discount/by-date-range
Retrieves discounts within a date range.

**Query Parameters**:
- startDate
- endDate

## GET /api/discount/by-starting-date
Retrieves discounts by starting date.

**Query Parameter**:
- startDate

## GET /api/discount/by-ending-date
Retrieves discounts by ending date.

**Query Parameter**:
- endDate

## POST /api/discount
Adds a new discount.

**Request Body**: 
- DiscountDTO

## PUT /api/discount/{discountId}
Updates a discount by ID.

**Request Body**: 
- DiscountDTO

## DELETE /api/discount/{discountId}
Deletes a discount.

## POST /api/discount/apply-to-product/{productId}/{discountId}
Applies a discount to a product.

## POST /api/discount/apply-to-category/{categoryName}/{discountId}
Applies a discount to a category.

## POST /api/discount/apply-to-brand/{brandName}/{discountId}
Applies a discount to a brand.

## POST /api/discount/remove-expired
Removes expired discounts.

#GenderController

## GET /api/gender/id/{GenderId}
Retrieves a gender by ID.

## GET /api/gender/name/{GenderName}
Retrieves a gender by name.

## GET /api/gender
Retrieves all genders.

## POST /api/gender
Adds a new gender.

**Request Body**:
- GenderName (string)

## PUT /api/gender/{GenderId}
Updates an existing gender.

**Request Body**:
- GenderName (string)

## DELETE /api/gender/{GenderId}
Deletes a gender.

#OrderController

## POST /api/order
Places a new order.

**Request Body**:
- OrderRequestDTO

## GET /api/order/{orderId}
Retrieves an order by ID.

## GET /api/order/user/{username}
Retrieves orders by a user’s username.

## GET /api/order
Retrieves all orders.

## GET /api/order/status/{status}
Retrieves orders by their status.

**Query Parameter**:
- status (OrderStatus)

## PUT /api/order/{orderId}/status
Updates the status of an order.

**Request Body**:
- OrderStatus

## POST /api/order/{orderId}/items
Adds an item to an existing order.

**Request Body**:
- OrderItemDTO

## DELETE /api/order/items/{orderId}/{orderItemId}
Removes an item from an order.

## PUT /api/order/{orderId}/cancel
Cancels an order.

#ProductController

## POST /api/product
Adds a new product.

**Request Body**: 
- ProductDTO

## GET /api/product/{productId}
Retrieves a product by ID.

## GET /api/product
Retrieves all products.

## GET /api/product/out-of-stock
Retrieves products that are out of stock.

## GET /api/product/brand/{brand}
Retrieves products by brand.

## GET /api/product/category/{category}
Retrieves products by category.

## GET /api/product/color/{color}
Retrieves products by color.

## GET /api/product/gender/{gender}
Retrieves products by gender.

## GET /api/product/size/{size}
Retrieves products by size.

## GET /api/product/with-discount
Retrieves products with discounts.

## PUT /api/product/{productId}
Updates an existing product.

**Request Body**: 
- ProductDTO

## DELETE /api/product/{productId}
Deletes a product.

## GET /api/product/quantity/{productId}
Retrieves real-time quantity information for a product.

## GET /api/product/search
Performs an advanced product search with multiple filters.

**Query Parameters**:
- category, gender, brand, minPrice, maxPrice, size, color, inStock

#ReportController

## GET /api/report
Retrieves all reports.

## GET /api/report/earnings/daily
Retrieves daily earnings.

**Query Parameter**:
- date (DateTime)

## GET /api/report/earnings/monthly
Retrieves monthly earnings.

**Query Parameters**:
- month (int), year (int)

## GET /api/report/earnings/total
Retrieves total earnings.

#SizeController

## GET /api/size/id/{SizeId}
Retrieves a size by ID.

## GET /api/size/name/{SizeName}
Retrieves a size by name.

## GET /api/size
Retrieves all sizes.

## POST /api/size
Adds a new size.

**Request Body**:
- SizeName (string)

## PUT /api/size/{SizeId}
Updates an existing size.

**Request Body**:
- SizeName (string)

## DELETE /api/size/{SizeId}
Deletes a size.

#UserController

## GET /api/user/username/{username}
Retrieves a user by username.

## GET /api/user/{userId}
Retrieves a user by user ID.

## PUT /api/user/{userId}
Updates an existing user.

**Request Body**:
- UserDTO

## GET /api/user
Retrieves all users.

## POST /api/user/reset-password
Resets a user's password.

**Request Body**:
- ResetPasswordDTO

## DELETE /api/user/{userId}
Deletes a user.

## GET /api/user/role/{roleName}
Retrieves users by their role.
