using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Requests;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly ITokenService _token;
        private readonly IMapper _map;


        public AccountController(DataContext db, ITokenService token, IMapper map)
        {
            _db = db;
            _token = token;
            _map = map;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register)
        {
            if (await UserExists(register.UserName)) return BadRequest("Username is taken");

            var user = _map.Map<AppUser>(register);

            using var hmac = new HMACSHA512();

            user.UserName = register.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            user.PasswordSalt = hmac.Key;

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = _token.CreateToken(user),
                Gender = user.Gender
            };
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName.ToUpper() == loginDTO.UserName.ToUpper());
            if (user is null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Token = _token.CreateToken(user),
                Gender = user.Gender
            };
        }


        private async Task<bool> UserExists(string username)
        {
            return await _db.Users.AnyAsync(x => x.UserName.ToUpper() == username.ToUpper());
        }


    }
}
