using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace BurgerManiaProject.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; }

        public DateTime OrderedOn { get; set; }

        

    }
}
