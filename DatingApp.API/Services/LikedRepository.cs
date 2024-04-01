using DatingApp.API.Controllers;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Extenstions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Services
{
    public class LikedRepository : ILikedRepository
    {
        private readonly DataContext _db;
        public LikedRepository(DataContext db)
        {
            _db = db;
        }

        public async Task<UserLike> GetUserLike(int SourceUserID, int TargetUserId)
        {
            return await _db.Likes.FindAsync(SourceUserID, TargetUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikedParams likedParams)
        {
            var user = _db.Users.OrderBy(x => x.UserName).AsQueryable();
            var likes = _db.Likes.AsQueryable();

            if(likedParams.Type == "liked")
            {
                likes = likes.Where(like => like.SourceUserID == likedParams.UserId);
                user = likes.Select(like => like.TargetUser);
            }
            else if(likedParams.Type == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserID == likedParams.UserId);
                user = likes.Select(like => like.SourceUser);
            }

            var likedUser = user.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.ID
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUser, likedParams.PageNumber, likedParams.PageSize);
            
        }

        public async Task<AppUser> GetUserWithLikes(int UserId)
        {
            return await _db.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.ID == UserId);
        }
    }
}
