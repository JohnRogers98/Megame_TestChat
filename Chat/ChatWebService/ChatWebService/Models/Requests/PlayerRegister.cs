using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public class PlayerRegister : RequestRegister
    {
        [Required]
        public String Nickname { get; set; }
    }
}