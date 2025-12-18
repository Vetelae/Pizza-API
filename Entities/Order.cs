using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Pizza_API.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public List<OrderItem> Items { get; set; } = new();
    }
}