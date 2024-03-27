using DatingApp.API.Extenstions;

namespace DatingApp.API.Entities
{
    public class AppUser
    {
        public int ID { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string KnownAs { get; set; }

        public string LookingFor { get; set; }

        public DateTime Created_On { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public List<Photo> Photos { get; set; } = new ();

        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
}
