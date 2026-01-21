using Microsoft.EntityFrameworkCore;
using Pizza_API.Data;
using Pizza_API.Entities;
using Pizza_API.Entities.Dtos.Pizza;


namespace Pizza_API.Services
{
    public class PizzaService : IPizzaService
    {
        private readonly ApplicationDbContext _dbContext;
        public PizzaService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<PizzaDto>> GetAllPizzasAsync()
        {
            return await _dbContext.Pizzas
                .Select(p => new PizzaDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Value = p.Value
                })
                .ToListAsync();
        }

        public async Task<PizzaDto?> GetPizzaByIdAsync(int id)
        {
            var pizza = await _dbContext.Pizzas.FindAsync(id);

            if (pizza == null) 
                return null;

            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value
            };
        }

        public async Task<PizzaDto> CreatePizzaAsync(CreatePizzaDto dto)
        {
            var pizza = new Pizza
            {
                Name = dto.Name,
                Value = dto.Value,
            };

            _dbContext.Pizzas.Add(pizza);
            await _dbContext.SaveChangesAsync();

            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value
            };
        }

        public async Task <PizzaDto?> UpdatePizzaAsync(int id, UpdatePizzaDto dto)
        {
            var pizza = await _dbContext.Pizzas.FindAsync(id);
            if (pizza == null) 
                return null;

            // Update the entity
            pizza.Name = dto.Name;
            pizza.Value = dto.Value;

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return updated DTO
            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value,
            };
        }

        public async Task <bool> DeletePizzaAsync(int id)
        {
            var pizza = await _dbContext.Pizzas.FindAsync(id);
            if (pizza == null) 
                return false;

            _dbContext.Pizzas.Remove(pizza);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}