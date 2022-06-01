using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public abstract class RequestLogin : RequestBase
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
    }
}
