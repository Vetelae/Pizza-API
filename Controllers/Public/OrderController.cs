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
                return BadRequest("Lookup token required");

            var order = await _orderService.GetOrderByIdAsync(id, token);
            return Ok(order);
        }
    }
}
