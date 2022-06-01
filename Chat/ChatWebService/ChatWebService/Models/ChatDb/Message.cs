using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebService.Models.ChatDb
{
    public class Message
    {
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.Identity)]
        public Int32 Id { get; set; }
        
        [ForeignKey("OperatorId")]
        public Operator Operator { get; set; }
        public Guid OperatorId { get; set; }
      
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public Guid PlayerId { get; set; }

        public Owner Owner { get; set; }

        public Boolean IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public String BodyMessage { get; set; }
    }

    public enum Owner
    {
        Operator,
        Player
    }
}
