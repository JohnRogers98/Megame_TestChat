using System.ComponentModel.DataAnnotations;

namespace ChatWebService.Models.Requests
{
    public class AdminRegister : RequestRegister
    {
        [Required]
        public String Name { get; set; }
    }
}