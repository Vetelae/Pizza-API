using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Enums;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OrderAdminController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderAdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: List of all orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // GET: Orders by status
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);

            return Ok(orders);
        }

        // GET: Order by id
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdForAdminAsync(id);
            return Ok(order);
        }

        // PUT: Update existing order
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, UpdateOrderDto dto)
        {
            var updated = await _orderService.UpdateOrderAsync(id, dto);
            return Ok(updated);
        }

        //DELETE: Delete existing order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _orderService.DeleteOrderAsync(id);

            return NoContent();
        }
    }
}