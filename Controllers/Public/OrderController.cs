using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/public/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Order by id
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Lookup token required");

            var order = await _orderService.GetOrderByIdAsync(id, token);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: Create new order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            // If user is authenticated, grab their UserId
            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            dto.UserId = userId; // Will be null for guests

            var createdOrder = await _orderService.CreateOrderAsync(dto);

            // If invalid items
            if (createdOrder == null)
                return BadRequest("Invalid menu items or unavailable items");

            return CreatedAtAction(nameof(GetOrderById),
                new { id = createdOrder.Id },
                createdOrder);
        }
    }
}