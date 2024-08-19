using System.ComponentModel.DataAnnotations;
namespace BurgerManiaProject.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string MobileNumber { get; set; }

        //navigation
        public ICollection<Cart>? Cart { get; set; }

    }
}
