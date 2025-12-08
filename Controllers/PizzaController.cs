using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pizza_API.Data;
using Pizza_API.Entities;
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
        public IActionResult GetAllPizzas() 
        {
            var pizzas = _pizzaService.GetAllPizzas();

            return Ok(pizzas);
        }

        // GET: Pizza by id
        [HttpGet("{id}")]
        public IActionResult GetPizzaById(int id) 
        {
            var pizza = _pizzaService.GetPizzaById(id);
            if (pizza == null)
                return NotFound();

            return Ok(pizza);
        }

        // POST: Create new pizza
        [HttpPost]
        public IActionResult CreatePizza(CreatePizzaDto dto)
        {
            var createdPizza = _pizzaService.CreatePizza(dto);

            return CreatedAtAction(nameof(GetPizzaById),
                new { id = createdPizza.Id },
                createdPizza);
        }

        // PUT: Update existing pizza
        [HttpPut("{id}")]
        public IActionResult UpdatePizza(int id, UpdatePizzaDto dto)
        {
            var updated = _pizzaService.UpdatePizza(id, dto);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: Delete existing pizza
        [HttpDelete("{id}")]
        public IActionResult DeletePizza(int id)
        {
            var deleted = _pizzaService.DeletePizza(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}