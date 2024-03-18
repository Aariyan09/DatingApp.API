using AutoMapper;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _map;
        public UsersController(IUserRepository userRepo,IMapper map)
        {
            _userRepo = userRepo;
            _map = map;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member_DTO>>> Get()
        {
            var users = await _userRepo.GetAllUsersAsync();

            var usersToReturn = _map.Map<IEnumerable<Member_DTO>>(users);
            return Ok(usersToReturn);
        }


        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<Member_DTO>> GetUserById(int id)
        {
            var user = await _userRepo.GetAllMembersAsync();
            return Ok(user);
        }


        [HttpGet("GetUserByName/{username}")]
        public async Task<ActionResult<Member_DTO>> GetUserByName(string username)
        {
            var user = await _userRepo.GetMemberByNameAsync(username);
            return user;
        }

    }
}
