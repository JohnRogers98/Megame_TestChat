using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public class Message : RequestBase
    {
        [Required]
        public String Body { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }
    }
}
