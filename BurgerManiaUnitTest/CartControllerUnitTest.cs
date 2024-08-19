using BurgerManiaProject.Controllers;
using BurgerManiaProject.Data;
using BurgerManiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CartsControllerTests
{
    private DbContextOptions<BurgerMgmtDbContext> _options;

    private BurgerMgmtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BurgerMgmtDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var context = new BurgerMgmtDbContext(options);
        SeedDatabase(context);
        return context;
    }

    private void SeedDatabase(BurgerMgmtDbContext context)
    {
        context.Carts.RemoveRange(context.Carts); 
        context.SaveChanges();

        var carts = new List<Cart>
        {
            new Cart { CartId = 1, UserId = 1, Ordered = false },
            new Cart { CartId = 2, UserId = 2, Ordered = false }
        };

        context.Carts.AddRange(carts);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetCartsByUser_Returns_Carts_For_User()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result = controller.GetCartsByUser(1);

            // Assert
            var carts = Assert.IsAssignableFrom<IEnumerable<Cart>>(result);
            Assert.Single(carts);
            Assert.Equal(1, carts.First().UserId);
        }
    }

    [Fact]
    public async Task GetActiveCart_Returns_Active_Cart()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result =  controller.GetActiveCart(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var cart = Assert.IsType<Cart>(actionResult.Value);
            Assert.Equal(1, cart.UserId);
            Assert.False(cart.Ordered);
        }
    }

    [Fact]
    public async Task GetActiveCart_Returns_NotFound_When_No_Active_Cart()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result =  controller.GetActiveCart(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task CreateCart_Creates_New_Cart()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result = await controller.CreateCart(3);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var cart = Assert.IsType<Cart>(actionResult.Value);
            Assert.Equal(3, cart.UserId);
            Assert.False(cart.Ordered);

            // Verify the cart was added to the in-memory database
            var createdCart = await context.Carts.FindAsync(cart.CartId);
            Assert.NotNull(createdCart);
        }
    }

    [Fact]
    public async Task PlaceOrder_Updates_Cart_As_Ordered()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result = controller.PlaceOrder(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var cart = await context.Carts.FindAsync(1);
            Assert.NotNull(cart);
            Assert.True(cart.Ordered);
        }
    }

    [Fact]
    public async Task PlaceOrder_Returns_NotFound_When_Cart_Does_Not_Exist()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartsController(context);

            // Act
            var result =  controller.PlaceOrder(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}