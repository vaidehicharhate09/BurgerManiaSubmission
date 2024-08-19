using BurgerManiaProject.Data;
using BurgerManiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly BurgerMgmtDbContext _context;
    private readonly IConfiguration _configuration;

    public UserController(BurgerMgmtDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

     [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    
    [HttpPost("{login}")]
    public async Task<ActionResult<User>> Login([FromBody] User loginUser)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.MobileNumber == loginUser.MobileNumber && u.Name == loginUser.Name);

        if (user != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("MobileNo", user.MobileNumber.ToString()),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signIn
                );
            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { Token = tokenValue, User = user });
            //return Ok(user);

        }

        return NotFound();
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize]
    [HttpGet("mobile/{mobileNumber}")]
    public async Task<ActionResult<User>> GetUserByMobileNumber(string mobileNumber)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(string mobileNumber, string name)
    {
        
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);

        if (existingUser != null)
        {
            return Conflict("User with this mobile number already exists.");
        }

        var user = new User { MobileNumber = mobileNumber, Name = name };

        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); 

            var cart = new Cart { UserId = user.UserId, Ordered=false }; 
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(); 

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest("An error occurred while creating the user.");
        }
    }
}
