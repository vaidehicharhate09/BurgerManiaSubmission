using System.ComponentModel.DataAnnotations;
namespace BurgerManiaProject.Models
{
    public class Burger
    {
        [Key]
        public int BurgerId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public decimal Price { get; set; }


    }
}
