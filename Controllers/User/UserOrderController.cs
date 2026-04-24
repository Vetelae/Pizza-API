using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Services;

namespace Pizza_API.Controllers.User
{
    [Route("api/user/orders")]
    [ApiController]
    [Authorize]
    public class UserOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public UserOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/user/orders - Get current user's order history
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        // GET: api/user/orders/{id} - Get specific order (only if belongs to user)
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetMyOrderById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var order = await _orderService.GetOrderByIdForUserAsync(id, userId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}