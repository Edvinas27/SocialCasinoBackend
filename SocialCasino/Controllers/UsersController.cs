using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCasino.JWT;
using SocialCasino.Models;
using SocialCasino.Services;

namespace SocialCasino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly TokenGenerator _tokenGenerator;
        private readonly IConfiguration _configuration;
        private readonly FakeCreditCardService _fakeCreditCardService;

        public UsersController(MyDbContext context, TokenGenerator tokenGenerator, IConfiguration configuration, FakeCreditCardService fakeCreditCardService)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _configuration = configuration;
            _fakeCreditCardService = fakeCreditCardService;
        }

        [HttpPost]
        [Route("Register")]
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
                var token = _tokenGenerator.GenerateToken(user.UserId);
                var secureReturn = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.IsActive,  
                    user.CreatedAt,
                    //this token will be deleted
                    Token = token
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
        
        [HttpGet]
        [Route("GetCurrentUser")]
        public IActionResult GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            var objUser = _context.Users.FirstOrDefault(data => data.UserId == userId);
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

        [HttpPut]
        [Route("AddBalance")]
        [Authorize]
        public IActionResult AddBalance(int id, decimal amount)
        {
            var userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            var objUser = _context.Users.FirstOrDefault(data => data.UserId == userId);
            if (objUser is not null)
            {
                objUser.Balance += amount;
                _context.SaveChanges();
                return Ok("Balance added successfully");
            }

            return NoContent();
        }

        // will be used for simulation
        [HttpGet]
        [Route("GenerateCreditCard")]
        public IActionResult GenerateCreditCard(decimal balance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fakeCreditCard = _fakeCreditCardService.GenerateCreditCard(balance);

            var newCreditCard = new CreditCard
            {
                CardNumber = fakeCreditCard.CardNumber,
                ExpiryMonth = fakeCreditCard.ExpiryMonth,
                ExpiryYear = fakeCreditCard.ExpiryYear,
                CVV = fakeCreditCard.CVV,
                Balance = fakeCreditCard.Balance
            };

            _context.CreditCards.Add(newCreditCard);

            var zeroBalanceCards = _context.CreditCards.Where(card => card.Balance == 0).ToList();
            _context.CreditCards.RemoveRange(zeroBalanceCards);

            _context.SaveChanges();
            
            return Ok(newCreditCard);
        }
    }
}
