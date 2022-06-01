using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public class RequestRegister : RequestBase
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
    }
}
