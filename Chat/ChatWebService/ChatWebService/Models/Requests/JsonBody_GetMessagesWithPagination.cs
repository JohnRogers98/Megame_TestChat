using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public class JsonBody_GetMessagesWithPagination
    {
        [Required]
        public string OperatorId { get; set; }

        [Required]
        public string PlayerId { get; set; }

        public int Page { get; set; } = -1;
    }
}
