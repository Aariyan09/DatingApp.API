namespace DatingApp.API.Helpers
{
    public class LikedParams : PaginationParams
    {
        public int UserId { get; set; }

        public string Type { get; set; }
    }
}
