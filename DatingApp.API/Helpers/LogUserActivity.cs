﻿using DatingApp.API.Extenstions;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userid = resultContext.HttpContext.User.GetUserID();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(int.Parse(userid));
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();
        }
    }
}
