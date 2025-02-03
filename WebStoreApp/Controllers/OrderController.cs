using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebStoreApp.Exceptions;

namespace WebStoreApp.Controllers
{
    /// <summary>
    /// Manages order operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/orders")]
    [ApiVersion("1.0")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Places a new order.
        /// </summary>
        /// <param name="orderDto">The order details.</param>
        /// <returns>The placed order details.</returns>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Places a new order")]
        [SwaggerResponse(201, "Order placed successfully", typeof(OrderDTO))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<OrderDTO>> PlaceOrder([FromBody, Required] OrderRequestDTO orderDto)
        {
            try
            {
                if (!ModelState.IsValid || orderDto == null)
                {
                    return BadRequest(new { message = "Invalid order data." });
                }

                var order = await _orderService.PlaceOrder(orderDto);

                if (order == null)
                {
                    return StatusCode(500, new { message = "Failed to place the order." });
                }

                return StatusCode(201, new { message = "Order placed successfully.", order });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while placing the order.", error = ex.Message });
            }
        }



        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The order details.</returns>
        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        [SwaggerOperation(Summary = "Gets an order by ID")]
        [SwaggerResponse(200, "Success", typeof(OrderResponseDTO))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<OrderResponseDTO>> GetOrderById([Required] int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new { message = "Invalid order ID." });
                }

                var order = await _orderService.GetOrderById(orderId);
                return Ok(order);
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the order.", error = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves all orders for a specific user.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A list of orders.</returns>
        [HttpGet("user/{username}")]
        [Authorize]
        [SwaggerOperation(Summary = "Gets orders by username")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<OrderResponseDTO>))]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "No orders found for the user")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByUserName([Required] string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest(new { message = "Username cannot be empty." });
                }

                var orders = await _orderService.GetOrdersByUserName(username);
                if (!orders.Any())
                {
                    return NotFound(new { message = "No orders found for the user." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving orders.", error = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,AdvancedUser")]
        [SwaggerOperation(Summary = "Gets all orders")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<OrderResponseDTO>))]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        /// <summary>
        /// Gets orders filtered by status.
        /// </summary>
        /// <param name="status">The status of orders to retrieve.</param>
        /// <returns>A list of orders matching the given status.</returns>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        [SwaggerOperation(Summary = "Gets orders by status")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<OrderResponseDTO>))]
        [SwaggerResponse(400, "Invalid status value")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "No orders found for the given status")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByStatus([Required] OrderStatus status)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrderStatus), status))
                {
                    return BadRequest(new { message = "Invalid order status." });
                }

                var orders = await _orderService.GetOrdersByStatus(status);
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found for the given status." });
                }

                return Ok(orders);
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving orders by status.", error = ex.Message });
            }
        }


        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="status">The new status.</param>
        /// <returns>A success message if updated.</returns>
        [HttpPut("{orderId}/status/{status}")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        [SwaggerOperation(Summary = "Updates order status")]
        [SwaggerResponse(204, "Order status updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> UpdateOrderStatus([Required] int orderId, [Required] OrderStatus status)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new { message = "Invalid order ID." });
                }

                await _orderService.UpdateOrderStatus(orderId, status);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while updating order status.", error = ex.Message });
            }
        }


        /// <summary>
        /// Adds an item to an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="orderItemDto">The item details to be added.</param>
        /// <returns>Success message or error response.</returns>
        [HttpPost("{orderId}/items")]
        [Authorize]
        [SwaggerOperation(Summary = "Adds an item to an order")]
        [SwaggerResponse(200, "Item added successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> AddOrderItem([Required] int orderId, [FromBody, Required] OrderItemDTO orderItemDto)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest(new { message = "Order ID must be a valid positive integer." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingOrder = await _orderService.GetOrderById(orderId);
                if (existingOrder == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                await _orderService.AddOrderItem(orderId, orderItemDto);
                return Ok(new { message = "Item added successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while adding the order item.", error = ex.Message });
            }
        }


        /// <summary>
        /// Removes an item from an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="orderItemId">The ID of the item to remove.</param>
        /// <returns>No content if successful, otherwise an error message.</returns>
        [HttpDelete("items/{orderId}/{orderItemId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Removes an item from an order")]
        [SwaggerResponse(204, "Item removed successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Order or item not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> RemoveOrderItem([Required] int orderId, [Required] int orderItemId)
        {
            try
            {
                if (orderId <= 0 || orderItemId <= 0)
                {
                    return BadRequest(new { message = "Order ID and Order Item ID must be valid positive integers." });
                }

                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                await _orderService.RemoveOrderItem(orderId, orderItemId);
                return NoContent();
            }
            catch (ServiceException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while removing the order item.", error = ex.Message });
            }
        }


        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to cancel.</param>
        /// <returns>A success message if canceled.</returns>
        [HttpPut("{orderId}/cancel")]
        [Authorize]
        [SwaggerOperation(Summary = "Cancels an order")]
        [SwaggerResponse(200, "Order canceled successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized - User is not authenticated")]
        [SwaggerResponse(403, "Forbidden - User does not have permission")]
        [SwaggerResponse(404, "Order not found")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> CancelOrder([Required] int orderId)
        {
            try
            {
                var existingOrder = await _orderService.GetOrderById(orderId);
                if (existingOrder == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                await _orderService.CancelOrder(orderId);
                return Ok(new { message = "Order has been canceled successfully." });
            }
            catch (ServiceException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while canceling the order.", error = ex.Message });
            }
        }

    }
}
