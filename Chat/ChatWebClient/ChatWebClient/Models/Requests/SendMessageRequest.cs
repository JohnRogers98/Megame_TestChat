using System.ComponentModel.DataAnnotations;

namespace ChatWebClient.Models.Requests
{
    public class SendMessageRequest
    {
        [Required]
        public String ReceiverId { get; set; }

        [Required]
        public String Body { get; set; }
    }
}
