﻿namespace DatingApp.API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string? CurrentUserName { get; set; }
        public string? Gender { get; set; }

        public int MinAge { get; set; } = 10;
        public int MaxAge { get; set; } = 100;


        public string OrderBy { get; set; } = "lastActive";
    }
}
