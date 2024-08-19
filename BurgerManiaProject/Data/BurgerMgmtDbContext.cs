using BurgerManiaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BurgerManiaProject.Data;
public class BurgerMgmtDbContext : DbContext
{
    public BurgerMgmtDbContext(DbContextOptions<BurgerMgmtDbContext> options) : base(options)
    {
        
    }
    public DbSet<Burger> Burgers { get; set; }
    public DbSet<CartItems> CartItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Burger>().HasData(
            new Burger { BurgerId = 1, Name="Cheesy Delicious", Price=100, Type="Veg" },
            new Burger { BurgerId = 2, Name="Cheesy Delicious", Price=200, Type="Non-Veg" },
            new Burger { BurgerId = 3, Name="Cheesy Delicious", Price=150, Type="Egg" },
            
            new Burger { BurgerId = 4, Name = "Cheesy Surprise", Price = 100, Type = "Veg" },
            new Burger { BurgerId = 5, Name = "Cheesy Surprise", Price = 200, Type = "Non-Veg" },
            new Burger { BurgerId = 6, Name = "Cheesy Surprise", Price = 150, Type = "Egg" },

            new Burger { BurgerId = 7, Name = "Cheesy Chilli", Price = 100, Type = "Veg" },
            new Burger { BurgerId = 8, Name = "Cheesy Chilli", Price = 200, Type = "Non-Veg" },
            new Burger { BurgerId = 9, Name = "Cheesy Chilli", Price = 150, Type = "Egg" },

            new Burger { BurgerId = 10, Name = "Cheesy Burger", Price = 100, Type = "Veg" },
            new Burger { BurgerId = 11, Name = "Cheesy Burger", Price = 200, Type = "Non-Veg" },
            new Burger { BurgerId = 12, Name = "Cheesy Burger", Price = 150, Type = "Egg" },

            new Burger { BurgerId = 13, Name = "Cheesy Bonanza", Price = 100, Type = "Veg" },
            new Burger { BurgerId = 14, Name = "Cheesy Bonanza", Price = 200, Type = "Non-Veg" },
            new Burger { BurgerId = 15, Name = "Cheesy Bonanza", Price = 150, Type = "Egg" }
        );
        //User-cart
        modelBuilder.Entity<User>()
            .HasMany(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

       
        //cart-cartItems
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci=>ci.CartId)
            .OnDelete(DeleteBehavior.Restrict);

        //burger-cartItems
        modelBuilder.Entity<CartItems>()
            .HasOne(ci => ci.Burger)
            .WithMany()
            .HasForeignKey(ci => ci.BurgerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
    

}
    
}