using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStoreApp.DTOs;
using WebStoreApp.Models;
using WebStoreApp.Services.Interfaces;

namespace WebStoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDTO>> PlaceOrder([FromBody] OrderRequestDTO orderDto)
        {
            var order = await _orderService.PlaceOrder(orderDto);
            return Ok();
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            return Ok(order);
        }

        [HttpGet("user/{username}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByUserName(string username)
        {
            var orders = await _orderService.GetOrdersByUserName(username);
            return Ok(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,AdvancedUser")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatus(status);
            return Ok(orders);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin, AdvancedUser")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus status)
        {
            await _orderService.UpdateOrderStatus(orderId, status);
            return NoContent();

        }

        [HttpPost("{orderId}/items")]
        [Authorize]
        public async Task<IActionResult> AddOrderItem(int orderId, [FromBody] OrderItemDTO orderItemDto)
        {
            await _orderService.AddOrderItem(orderId, orderItemDto);
                return Ok();
        }

        [HttpDelete("items/{orderId}/{orderItemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveOrderItem(int orderId, int orderItemId)
        {
            await _orderService.RemoveOrderItem(orderId, orderItemId);
            return NoContent();
        }

        [HttpPut("{orderId}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                await _orderService.CancelOrder(orderId);
                return Ok(new { message = "Order has been canceled successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while canceling the order.", error = ex.Message });
            }
        }

    }
}
