using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            var cart = await _cartService.GetCartAsync(userId, sessionId);

            // Return empty cart response instead of 404
            if (cart == null)
                return Ok(new CartDto());

            return Ok(cart);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddItem(AddCartItemDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            if (userId == null && sessionId == null)
                return BadRequest("Session ID or authentication required");

            var cart = await _cartService.AddItemToCartAsync(userId, sessionId, dto);

            if (cart == null)
                return BadRequest("Menu item not found or unavailable");

            return Ok(cart);
        }

        // PUT: api/cart/items/{cartItemId}
        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> UpdateItem(int cartItemId, UpdateCartItemDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            if (userId == null && sessionId == null)
                return BadRequest("Session ID or authentication required");

            var cart = await _cartService.UpdateCartItemAsync(userId, sessionId, cartItemId, dto);

            if (cart == null)
                return NotFound("Cart or item not found");

            return Ok(cart);
        }

        // DELETE: api/cart/items/{cartItemId}
        [HttpDelete("items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> RemoveItem(int cartItemId)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            if (userId == null && sessionId == null)
                return BadRequest("Session ID or authentication required");

            var cart = await _cartService.RemoveCartItemAsync(userId, sessionId, cartItemId);

            if (cart == null)
                return NotFound("Cart or item not found");

            return Ok(cart);
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            if (userId == null && sessionId == null)
                return BadRequest("Session ID or authentication required");

            var result = await _cartService.ClearCartAsync(userId, sessionId);

            if (!result)
                return NotFound("Cart not found");

            return NoContent();
        }

        // POST: api/cart/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout(CheckoutDto dto)
        {
            var (userId, sessionId) = CartIdentifierHelper.GetCartIdentifiers(HttpContext);

            if (userId == null && sessionId == null)
                return BadRequest("Session ID or authentication required");

            var order = await _cartService.CheckoutAsync(userId, sessionId, dto);

            if (order == null)
                return BadRequest("Checkout failed. Cart may be empty or items unavailable");

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