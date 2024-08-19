using BurgerManiaProject.Controllers;
using BurgerManiaProject.Data;
using BurgerManiaProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class UserControllerTests
{
    private DbContextOptions<BurgerMgmtDbContext> _options;

    // Initializes the context with an in-memory database
    private BurgerMgmtDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BurgerMgmtDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure unique database for each test
            .Options;

        var context = new BurgerMgmtDbContext(options);
        SeedDatabase(context);
        return context;
    }

    // Seeds initial data for the tests
    private void SeedDatabase(BurgerMgmtDbContext context)
    {
        context.Users.RemoveRange(context.Users); // Clear existing data
        context.Carts.RemoveRange(context.Carts); // Clear existing data
        context.SaveChanges();

        var users = new List<User>
        {
            new User { UserId = 1, MobileNumber = "1234567890", Name = "John Doe" },
            new User { UserId = 2, MobileNumber = "0987654321", Name = "Jane Smith" }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_Returns_All_Users()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);

            // Act
            var result = await controller.GetUsers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(actionResult.Value);
            Assert.Equal(2, users.Count());
        }
    }

    [Fact]
    public async Task Login_Returns_User_When_Credentials_Are_Correct()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);
            var loginUser = new User { MobileNumber = "1234567890", Name = "John Doe" };

            // Act
            var result = await controller.Login(loginUser);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var user = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(1, user.UserId);
        }
    }

    [Fact]
    public async Task Login_Returns_NotFound_When_Credentials_Are_Incorrect()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);
            var loginUser = new User { MobileNumber = "1234567890", Name = "Wrong Name" };

            // Act
            var result = await controller.Login(loginUser);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task GetUserById_Returns_User_When_Id_Is_Valid()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);

            // Act
            var result = await controller.GetUserById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var user = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(1, user.UserId);
        }
    }

    [Fact]
    public async Task GetUserById_Returns_NotFound_When_Id_Is_Invalid()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);

            // Act
            var result = await controller.GetUserById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

   

    [Fact]
    public async Task GetUserByMobileNumber_Returns_NotFound_When_MobileNumber_Is_Invalid()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);

            // Act
            var result = await controller.GetUserByMobileNumber("0000000000");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    

    [Fact]
    public async Task CreateUser_Returns_Conflict_When_User_Already_Exists()
    {
        // Arrange
        using (var context = CreateContext())
        {
            var controller = new UserController(context);
            var mobileNumber = "1234567890"; // Already exists in seeded data

            // Act
            var result = await controller.CreateUser(mobileNumber, "New Name");

            // Assert
            Assert.IsType<ConflictObjectResult>(result.Result);
        }
    }
}