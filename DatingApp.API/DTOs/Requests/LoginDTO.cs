using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs.Requests
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
