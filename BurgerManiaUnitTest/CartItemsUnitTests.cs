using BurgerManiaProject.Controllers;
using BurgerManiaProject.Data;
using BurgerManiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CartItemControllerTests
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
        context.CartItems.RemoveRange(context.CartItems); 
        context.SaveChanges();

        var cartItems = new List<CartItems>
        {
            new CartItems { CartItemId = 1, BurgerId = 1, Quantity = 2 },
            new CartItems { CartItemId = 2, BurgerId = 2, Quantity = 1 }
        };

        context.CartItems.AddRange(cartItems);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetCartItems_Returns_All_CartItems()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartItemController(context);

            // Act
            var result = await controller.GetCartItems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CartItems>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CartItems>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }
    }

    [Fact]
    public async Task GetCartItem_Returns_CartItem_By_Id()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartItemController(context);

            // Act
            var result = await controller.GetCartItem(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CartItems>>(result);
            var model = Assert.IsAssignableFrom<CartItems>(actionResult.Value);
            Assert.Equal(1, model.CartItemId);
        }
    }

    [Fact]
    public async Task GetCartItem_Returns_NotFound_When_CartItem_Does_Not_Exist()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartItemController(context);

            // Act
            var result = await controller.GetCartItem(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }

    [Fact]
    public async Task AddCartItem_Creates_New_CartItem()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartItemController(context);
            var newItem = new CartItems { CartItemId = 3, BurgerId = 3, Quantity = 1 };

            // Act
            var result = await controller.AddCartItem(newItem);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CartItems>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var model = Assert.IsAssignableFrom<CartItems>(createdAtActionResult.Value);
            Assert.Equal(3, model.CartItemId);

            var addedItem = await context.CartItems.FindAsync(3);
            Assert.NotNull(addedItem);
        }
    }

    [Fact]
    public async Task DeleteCartItem_Removes_CartItem()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new CartItemController(context);

            // Act
            var result = await controller.DeleteCartItem(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deletedItem = await context.CartItems.FindAsync(1);
            Assert.Null(deletedItem);
        }
    }
}
