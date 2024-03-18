using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByNameAsync(string name);

        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<IEnumerable<Member_DTO>> GetAllMembersAsync();

        Task<Member_DTO> GetMemberByNameAsync(string name);
    }
}
