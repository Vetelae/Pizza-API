using System.ComponentModel.DataAnnotations;
using Pizza_API.Enums;

namespace Pizza_API.Entities.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus? Status { get; set; }
    }
}
