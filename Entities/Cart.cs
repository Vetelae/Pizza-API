namespace Pizza_API.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string? UserId { get; set; }  // Nullable for guests
        public string? SessionId { get; set; }  // For guest users
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser? User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}