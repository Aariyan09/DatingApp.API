using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;
        private readonly IMapper _map;

        public UserRepository(DataContext db, IMapper map)
        {
            _db = db;
            _map = map;
        }

        public async Task<IEnumerable<Member_DTO>> GetAllMembersAsync()
        {
            return await _db.Users.ProjectTo<Member_DTO>(_map.ConfigurationProvider).ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await _db.Users.Include(x => x.Photos).ToListAsync();
        }

        public async Task<Member_DTO> GetMemberByNameAsync(string name)
        {
            return await _db.Users.Where(x => x.UserName == name).ProjectTo<Member_DTO>(_map.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
            return await _db.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName.ToUpper() == name.ToUpper());
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _db.Entry(user).State = EntityState.Modified;
        }
    }
}
