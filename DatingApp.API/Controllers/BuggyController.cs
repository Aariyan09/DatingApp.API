using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly DataContext _db;
        public BuggyController(DataContext context)
        {
            _db = context;
        }

        [Authorize]
        [HttpGet("Auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret-test";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _db.Users.Find(-1);
            if (thing is null) return NotFound();

            else return thing;
        }


        [HttpGet("server-error")]
        public ActionResult<string> GetServerError() 
        {
            var thing = _db.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }


        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("this is a bad request");
        }


    }
}
