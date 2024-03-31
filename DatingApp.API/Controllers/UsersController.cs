using AutoMapper;
using DatingApp.API.DTOs.Requests;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Extenstions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _map;
        private readonly IPhotoService _photo;
        public UsersController(IUserRepository userRepo, IMapper map, IPhotoService photo)
        {
            _userRepo = userRepo;
            _map = map;
            _photo = photo;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member_DTO>>> Get()
        {
            var users = await _userRepo.GetAllUsersAsync();

            var usersToReturn = _map.Map<IEnumerable<Member_DTO>>(users);
            return Ok(usersToReturn);
        }


        [HttpGet("GetUsers")]
        public async Task<ActionResult<PagedList<Member_DTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            var currentuser = await _userRepo.GetUserByNameAsync(User.GetUserName());
            userParams.CurrentUserName = currentuser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender)) 
            {
                userParams.Gender = currentuser.Gender == "male" ? "Female" : "male";
            } 

            var user = await _userRepo.GetAllMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(user.CurrentPage,user.PageSize,user.TotalCount,user.TotalPages));
            return Ok(user);
        }


        [HttpGet("GetUserByName/{username}")]
        [ServiceFilter(typeof(LogUserActivity))]
        public async Task<ActionResult<Member_DTO>> GetUserByName(string username)
        {
            var user = await _userRepo.GetMemberByNameAsync(username);
            return user;
        }



        [HttpPut]
        public async Task<ActionResult> UpdateUsers(MemberUpdate_DTO membersupdate_dto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepo.GetUserByNameAsync(username);
            if (user is null) return NotFound();

            _map.Map(membersupdate_dto, user);
            if (await _userRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }




        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo_DTO>> AddPhot(IFormFile file)
        {
            var user = await _userRepo.GetUserByNameAsync(User.GetUserName());
            if (user is null) return NotFound();

            var result = await _photo.AddPhotoAsync(file);

            if (result.Error is not null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUri.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count <= 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userRepo.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUserByName), new { username = User.GetUserName() },_map.Map<Photo_DTO>(photo));
            }

            return BadRequest("Something went wrong while adding photo");

        }



        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMain(string photoId)
        {
            var user = await _userRepo.GetUserByNameAsync(User.GetUserName());
            if (user is null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id.ToString() == photoId);
            if (photo is null) return NotFound();

            if (photo.IsMain) return BadRequest("The photo is always set as main photo");

            var currMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currMain is not null) currMain.IsMain = false;

            photo.IsMain = true;

            if (await _userRepo.SaveAllAsync()) return NoContent();

            return BadRequest("Error while setting photo as main");
        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(string photoId)
        {
            var user = await _userRepo.GetUserByNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id.ToString() == photoId);

            if(photo is null) return NotFound();
            if(photo.IsMain) return NotFound("Can't delete main photo");

            if(photo.PublicId is not null)
            {
                var result = await _photo.DeletePhotoAsync(photo.PublicId);

                if (result.Error is not null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _userRepo.SaveAllAsync()) return Ok();

            return BadRequest("Something went wrong while deleting photo");
        }

    }
}
