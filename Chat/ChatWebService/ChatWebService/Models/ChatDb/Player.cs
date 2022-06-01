using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.ChatDb
{
    public class Player
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public String Email { get; set; }

        public IList<PlayerConnection> PlayerConnections { get; set; } 
            = new List<PlayerConnection>();

        public IList<Message> Messages { get; set; } 
            = new List<Message>();
    }
}