using Microsoft.AspNetCore.Mvc;
using Pizza_API.Entities.Dtos.Pizza;
using Pizza_API.Services;

namespace Pizza_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly IPizzaService _pizzaService;

        public PizzaController(IPizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }

        // GET: List of all pizzas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaDto>>> GetAllPizzas() 
        {
            var pizzas = await _pizzaService.GetAllPizzasAsync();

            return Ok(pizzas);
        }

        // GET: Pizza by id
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaDto>> GetPizzaById(int id) 
        {
            var pizza = await _pizzaService.GetPizzaByIdAsync(id);
            if (pizza == null)
                return NotFound();

            return Ok(pizza);
        }

        // POST: Create new pizza
        [HttpPost]
        public async Task<ActionResult<PizzaDto>> CreatePizza(CreatePizzaDto dto)
        {
            var createdPizza = await _pizzaService.CreatePizzaAsync(dto);

            if (createdPizza == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetPizzaById),
                new { id = createdPizza.Id },
                createdPizza);
        }

        // PUT: Update existing pizza
        [HttpPut("{id}")]
        public async Task<ActionResult<PizzaDto>> UpdatePizza(int id, UpdatePizzaDto dto)
        {
            var updated = await _pizzaService.UpdatePizzaAsync(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: Delete existing pizza
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizza(int id)
        {
            var deleted = await _pizzaService.DeletePizzaAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}