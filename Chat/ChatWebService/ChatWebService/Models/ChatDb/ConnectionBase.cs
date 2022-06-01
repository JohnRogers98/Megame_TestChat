using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.ChatDb
{
    public abstract class ConnectionBase
    {
        [Key]
        public String ConnectionId { get; set; } 
    }
}
