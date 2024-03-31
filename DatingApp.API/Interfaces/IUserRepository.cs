using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByNameAsync(string name);

        Task<IEnumerable<AppUser>> GetAllUsersAsync();

        Task<PagedList<Member_DTO>> GetAllMembersAsync(UserParams userParams);

        Task<Member_DTO> GetMemberByNameAsync(string name);
    }
}
