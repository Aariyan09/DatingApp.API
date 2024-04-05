using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Requests;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IMapper _map;
        private readonly UserManager<AppUser> _userManager;


        public AccountController(UserManager<AppUser> userManager, ITokenService token, IMapper map)
        {
            _userManager = userManager;
            _token = token;
            _map = map;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register)
        {
            if (await UserExists(register.UserName)) return BadRequest("Username is taken");

            var user = _map.Map<AppUser>(register);

            user.UserName = register.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault());

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            return new UserDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = await _token.CreateToken(user),
                Gender = user.Gender
            };
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.ToUpper() == loginDTO.UserName.ToUpper());

            if (user is null) return Unauthorized("Invalid username");

            var result = await _userManager.CheckPasswordAsync(user,loginDTO.Password);
            if (!result) return Unauthorized("Invalid password");

            return new UserDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = await _token.CreateToken(user),
                Gender = user.Gender
            };
        }


        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName.ToUpper() == username.ToUpper());
        }


    }
}
