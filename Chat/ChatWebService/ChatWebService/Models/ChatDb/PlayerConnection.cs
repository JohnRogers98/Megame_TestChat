using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebService.Models.ChatDb
{
    public class PlayerConnection : ConnectionBase
    {
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public Guid PlayerId { get; set; }
    }
}
