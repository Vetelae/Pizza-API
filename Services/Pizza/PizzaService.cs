using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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

        public List<PizzaDto> GetAllPizzas()
        {
            return _dbContext.Pizzas
                .Select(p => new PizzaDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Value = p.Value
                })
                .ToList();
        }

        public PizzaDto? GetPizzaById(int id)
        {
            var pizza = _dbContext.Pizzas.Find(id);

            if (pizza == null) 
                return null;

            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value
            };
        }

        public PizzaDto CreatePizza(CreatePizzaDto dto)
        {
            var pizza = new Pizza
            {
                Name = dto.Name,
                Value = dto.Value,
            };

            _dbContext.Pizzas.Add(pizza);
            _dbContext.SaveChanges();

            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value
            };
        }

        public PizzaDto? UpdatePizza(int id, UpdatePizzaDto dto)
        {
            var pizza = _dbContext.Pizzas.Find(id);
            if (pizza == null) 
                return null;

            // Update the entity
            pizza.Name = dto.Name;
            pizza.Value = dto.Value;

            // Save changes
            _dbContext.SaveChanges();

            // Return updated DTO
            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Value = pizza.Value,
            };
        }

        public bool DeletePizza(int id)
        {
            var pizza = _dbContext.Pizzas.Find(id);
            if (pizza == null) 
                return false;

            _dbContext.Pizzas.Remove(pizza);
            _dbContext.SaveChanges();
            return true;
        }
    }
}