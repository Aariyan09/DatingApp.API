using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Extenstions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class LikesController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ILikedRepository _likesRepo;

        public LikesController(IUserRepository userRepo, ILikedRepository likesRepo)
        {
            _userRepo = userRepo;
            _likesRepo = likesRepo;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = int.Parse(User.GetUserID());
            var likedUser = await _userRepo.GetUserByNameAsync(username);

            var sourceUser = await _likesRepo.GetUserWithLikes(sourceUserId);

            if (likedUser is null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepo.GetUserLike(sourceUserId,likedUser.Id);
            if (userLike is not null) return BadRequest("You already liked this user");

            userLike = new UserLike
            {
                SourceUserID = sourceUserId,
                TargetUserID = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepo.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery] LikedParams likeParams)
        {
            likeParams.UserId = int.Parse(User.GetUserID());
            var user = await _likesRepo.GetUserLikes(likeParams);

            Response.AddPaginationHeader(new PaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount, user.TotalPages));
            return Ok(user);
        }
    }
}
