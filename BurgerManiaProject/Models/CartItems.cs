using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BurgerManiaProject.Models
{
    public class CartItems
    {
        [Key]
        public  int  CartItemId { get; set; }
        public  int  CartId { get; set; }
        public int UserId { get; set; }
        public int BurgerId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        
        public Burger? Burger { get; set; }
        [JsonIgnore]
        public Cart? Cart { get; set; }
   }
}
