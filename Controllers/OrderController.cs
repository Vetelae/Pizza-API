using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Order;
using Pizza_API.Services;

namespace Pizza_API.Controllers
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

        // GET: List of all orders
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _orderService.GetAllOrders();

            return Ok(orders);
        }

        // GET: Order by id
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: Create new order
        [HttpPost]
        public IActionResult CreateOrder(CreateOrderDto dto)
        {
            var createdOrder = _orderService.CreateOrder(dto);

            // If invalid pizza id
            if (createdOrder == null)
                return NotFound();

            return CreatedAtAction(nameof(GetOrderById),
                new { id = createdOrder.Id },
                createdOrder);
        }

        // PUT: Update existing order
       [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, UpdateOrderDto dto)
        {
            var updated = _orderService.UpdateOrder(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        //DELETE: Delete existing order
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var deleted = _orderService.DeleteOrder(id);
    
            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}