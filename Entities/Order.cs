using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Pizza_API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int PizzaId { get; set; }

        public Pizza Pizza { get; set; }
        public int Quantity { get; set; }
    }
}
