using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCasino.Models;

namespace SocialCasino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objUser = _context.Users.FirstOrDefault(data => data.Email == userDto.Email);
            
            if (objUser is not null)
            {
                return BadRequest("User already exists");
            }

            _context.Users.Add(new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Password = userDto.Password
            });
            _context.SaveChanges();
            return Ok("User registered successfully");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(data => data.Email == loginDto.Email && data.Password == loginDto.Password && data.IsActive);

            if (user is not null)
            {
                var secureReturn = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.IsActive,
                    user.CreatedAt
                };
                return Ok(secureReturn);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser(int id)
        {
            var objUser = _context.Users.FirstOrDefault(data => data.UserId == id);
            if (objUser is not null)
            {
                return Ok(objUser);
            }
            return NoContent();
        }

        [HttpPut]
        [Route("BanUser")]
        public IActionResult BanUser(int id)
        {
            var objUser = _context.Users.FirstOrDefault(data => data.UserId == id);
            if (objUser is not null)
            {
                objUser.IsActive = !objUser.IsActive;
                _context.SaveChanges();
                if (objUser.IsActive)
                {
                    return Ok("User unbanned successfully");
                }
                return Ok("User banned successfully");
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var objUser = _context.Users.FirstOrDefault(data => data.UserId == id);
            if (objUser is not null)
            {
                _context.Users.Remove(objUser);
                _context.SaveChanges();
                return Ok("User deleted successfully");
            }
            return NoContent();
        }
    }
}
