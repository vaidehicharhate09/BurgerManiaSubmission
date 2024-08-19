using Microsoft.AspNetCore.Mvc;
using BurgerManiaProject.Models;
using BurgerManiaProject.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BurgerManiaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly BurgerMgmtDbContext _context;

        public CartsController(BurgerMgmtDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts/user/{userId}
        [HttpGet("user/{userId}")]
        public IEnumerable<Cart> GetCartsByUser(int userId)
        {
            return _context.Carts.Where(c => c.UserId == userId);
        }

        // GET: api/Carts/user/{userId}/active
        [HttpGet("user/{userId}/active")]
        public IActionResult GetActiveCart(int userId)
        {
            var cart =  _context.Carts.FirstOrDefault(c => c.UserId == userId && !c.Ordered);
            if (cart == null) { 
                return NotFound();
            }
            return Ok(cart);
        }

        [HttpPost("user/{userId}/create")]
        public async Task<ActionResult<Cart>> CreateCart(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                Ordered = false,
            };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActiveCart), new { userId= userId }, cart);
        }

        
        // POST: api/Carts/{id}/order
        [HttpPost("{id}/order")]
        public IActionResult PlaceOrder(int id)
        {
            var cart = _context.Carts
                .Include(c=>c.CartItems)
                .FirstOrDefault(c=>c.UserId==id && !c.Ordered);

            if (cart == null) {
                return NotFound("Active Cart not found");
            }

            cart.Ordered = true;
            _context.SaveChanges();

            return Ok("Order placed successfully");
        }
    }
}