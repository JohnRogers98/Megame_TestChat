using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebService.Models.ChatDb
{
    public class OperatorConnection : ConnectionBase
    {
        [ForeignKey("OperatorId")]
        public Operator Operator { get; set; }
        public Guid OperatorId { get; set; }
    }
}
