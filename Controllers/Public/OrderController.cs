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
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: Create new order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(dto);

            // If invalid menuitem id
            if (createdOrder == null)
                return BadRequest("One or more menu items are invalid.");

            return CreatedAtAction(nameof(GetOrderById),
                new { id = createdOrder.Id },
                createdOrder);
        }
    }
}