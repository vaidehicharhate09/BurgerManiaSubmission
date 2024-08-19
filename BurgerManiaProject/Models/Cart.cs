using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BurgerManiaProject.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int  UserId { get; set; }
        public bool Ordered { get; set; }

        //Navigation
        public ICollection<CartItems> CartItems { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

    }
}
