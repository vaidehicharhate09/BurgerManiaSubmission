using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BurgerManiaProject.Data;
using BurgerManiaProject.Models;


namespace BurgerManiaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BurgersController : ControllerBase
    {
        private readonly BurgerMgmtDbContext _context;

        public BurgersController(BurgerMgmtDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Burger>>> GetBurgers()
        {
            return await _context.Burgers.ToListAsync();
        }

        [HttpGet("getByNameAndType")]
        public async Task<ActionResult<Burger>> GetBurgerId(string name, string type)
        {
            var burger = await _context.Burgers.FirstOrDefaultAsync(b=>b.Name==name && b.Type==type);

            if (burger == null)
            {
                return NotFound();
            }

            return Ok(burger);
        }

        [HttpGet("{burgerId}")]
        public async Task<ActionResult<Burger>> GetBurger(int burgerId)
        {
            var burger = await _context.Burgers.FindAsync(burgerId);

            if (burger == null)
            {
                return NotFound();
            }

            return Ok(burger);
        }

        private bool BurgerExists(int id)
        {
            return _context.Burgers.Any(e => e.BurgerId == id);
        }
    }
}
