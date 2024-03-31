using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Data;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
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

        public async Task<PagedList<Member_DTO>> GetAllMembersAsync(UserParams userParams)
        {
            var query = _db.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUserName);
            query = query.Where(x => x.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge - 1);

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created_On),
                       _  => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<Member_DTO>.CreateAsync(query.AsNoTracking().ProjectTo<Member_DTO>(_map.ConfigurationProvider),
                                                            userParams.PageNumber,userParams.PageSize);
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
