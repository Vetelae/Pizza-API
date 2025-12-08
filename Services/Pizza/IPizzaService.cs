using Pizza_API.Entities.Dtos.Pizza;

namespace Pizza_API.Services
{
    public interface IPizzaService
    {
        List<PizzaDto> GetAllPizzas();
        PizzaDto? GetPizzaById(int id);
        PizzaDto CreatePizza(CreatePizzaDto dto);
        PizzaDto? UpdatePizza(int id, UpdatePizzaDto dto);
        bool DeletePizza(int id);
    }
}