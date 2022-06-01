using System.ComponentModel.DataAnnotations;

namespace ChatWebClient.Models.Responses
{
    public class Message
    {
        [Required]
        public Int32 Id { get; set; }

        [Required]
        public String OperatorId { get; set; }
        [Required]
        public String PlayerId { get; set; }

        [Required]
        public Owner Owner { get; set; }

        [Required]
        public Boolean IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public String BodyMessage { get; set; }
    }

    public enum Owner
    {
        Operator,
        Player
    }
}
