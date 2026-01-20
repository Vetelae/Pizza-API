using Pizza_API.Entities.Dtos.Pizza;

namespace Pizza_API.Services
{
    public interface IPizzaService
    {
        Task<List<PizzaDto>> GetAllPizzasAsync();
        Task<PizzaDto?> GetPizzaByIdAsync(int id);
        Task<PizzaDto> CreatePizzaAsync(CreatePizzaDto dto);
        Task<PizzaDto?> UpdatePizzaAsync(int id, UpdatePizzaDto dto);
        Task<bool> DeletePizzaAsync(int id);
    }
}