using DatingApp.API.Entities;
using DatingApp.API.Extenstions;

namespace DatingApp.API.DTOs.Response
{
    public class Member_DTO
    {
        public int ID { get; set; }

        public string UserName { get; set; }
        public string PhotoUrl { get; set; }

        public int Age { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created_On { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public List<Photo_DTO> Photos { get; set; } = new();

    }
}
