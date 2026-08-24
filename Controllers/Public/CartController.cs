using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Pizza_API.Constants;
using Pizza_API.Entities.Dtos.Cart;
using Pizza_API.Entities.Dtos.CartItem;
using Pizza_API.Helpers;
using Pizza_API.Services;

namespace Pizza_API.Controllers.Public
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]
        [EnableRateLimiting(RateLimitPolicies.CartReads)]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var cart = await _cartService.GetCartAsync(userId, sessionId);

            return Ok(cart);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        [EnableRateLimiting(RateLimitPolicies.CartMutations)]
        public async Task<ActionResult<CartDto>> AddItem(AddCartItemDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var cart = await _cartService.AddItemToCartAsync(userId, sessionId, dto);

            return Ok(cart);
        }

        // PUT: api/cart/items/{cartItemId}
        [HttpPut("items/{cartItemId}")]
        [EnableRateLimiting(RateLimitPolicies.CartMutations)]
        public async Task<ActionResult<CartDto>> UpdateItem(int cartItemId, UpdateCartItemDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var cart = await _cartService.UpdateCartItemAsync(userId, sessionId, cartItemId, dto);

            return Ok(cart);
        }

        // DELETE: api/cart/items/{cartItemId}
        [HttpDelete("items/{cartItemId}")]
        [EnableRateLimiting(RateLimitPolicies.CartMutations)]
        public async Task<ActionResult<CartDto>> RemoveItem(int cartItemId)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var cart = await _cartService.RemoveCartItemAsync(userId, sessionId, cartItemId);

            return Ok(cart);
        }

        // DELETE: api/cart
        [HttpDelete]
        [EnableRateLimiting(RateLimitPolicies.CartMutations)]
        public async Task<ActionResult> ClearCart()
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            await _cartService.ClearCartAsync(userId, sessionId);

            return NoContent();
        }

        // POST: api/cart/checkout
        [HttpPost("checkout")]
        [EnableRateLimiting(RateLimitPolicies.Checkout)]
        public async Task<ActionResult> Checkout(CheckoutDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var order = await _cartService.CheckoutAsync(userId, sessionId, dto);

            // Return the created order
            return CreatedAtAction(
                "GetOrderById",           // Action name
                "Order",                  // Controller name
                new { id = order.Id },    // Route values
                order                     // Response body
            );
        }
    }
}
