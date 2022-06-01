using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.ChatDb
{
    public class Operator
    {
        public Guid Id { get; set; }

        [Required]
        public String Email { get; set; }

        public IList<OperatorConnection> OperatorConnections { get; set; } 
            = new List<OperatorConnection>();

        public IList<Message> Messages { get; set; } 
            = new List<Message>();
    }
}