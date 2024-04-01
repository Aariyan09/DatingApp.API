using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Interfaces
{
    public interface ILikedRepository
    {
        Task<UserLike> GetUserLike(int SourceUserID, int TargetUserId);
        Task<AppUser> GetUserWithLikes(int UserId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikedParams @params);

    }
}
