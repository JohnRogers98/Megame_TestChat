using System.ComponentModel.DataAnnotations;

namespace ChatWebClient.Models.Responses
{
    public class LoginResponse
    {
        [Required]
        public String Token { get; set; }

        [Required]
        public String Id { get; set; }
    }
}
