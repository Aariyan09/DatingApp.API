using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatingApp.API.Extenstions
{
    public static class IdentityService
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;

            }).AddRoles<AppRole>()
              .AddRoleManager<RoleManager<AppRole>>()
              .AddEntityFrameworkStores<DataContext>();

            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT")["Key"])),
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            service.AddAuthorization(options =>
            {
                options.AddPolicy("RequiredAdminRole", pol => pol.RequireRole("Admin"));
            });

            return service;
        }
    }
}
