using BurgerManiaProject.Data;
using BurgerManiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CartItemController : ControllerBase
{
    private readonly BurgerMgmtDbContext _context;

    public CartItemController(BurgerMgmtDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItems>>> GetCartItems()
    {
        return await _context.CartItems
            .Include(ci=>ci.Burger)
            .ToListAsync();
    }

    // POST: api/CartItem
    [HttpPost]
    public async Task<ActionResult<CartItems>> AddCartItem([FromBody] CartItems cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.CartItemId }, cartItem);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartItems>> GetCartItem(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
        {
            return NotFound("CartItem not found.");
        }

        return Ok(cartItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCartItem(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null) {
            return NotFound();
        }
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
