using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewsAppExample.Model;
using NewsAppExample.DTO;
using NewsAppExample.Helper;
using System.Security.Cryptography;

namespace NewsAppExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly NewsContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHashService _passwordHashService;

        public LoginController(NewsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            //_passwordHashService = passwordHashService;
            _passwordHashService = new PasswordHashService(); 
        }





















        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            //if (!modelstate.isvalid)
            //{
            //    return BadRequest("Invalid login request");
            //}

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == userLoginDto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var hashedPassword = _passwordHashService.IterateHash(userLoginDto.Password, user.Salt);



            if (user.PasswordHash != hashedPassword)
            {
                return Unauthorized("Invalid username or password");
            }

            //JwtService jwtService = new JwtService();
            //var token = jwtService.GenerateToken(user.UserId);


            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigReader.getJwtKey());

            //_configuration["Jwt:Secret"]

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser(UserLoginDto userLoginDto)
        {
            string salt = System.Convert.ToBase64String(RandomNumberGenerator.GetBytes(50));

            string hashedPw = _passwordHashService.IterateHash(userLoginDto.Password, salt);
            string username = userLoginDto.Username;


            User user = new User
            {
                PasswordHash = hashedPw,
                Username = username,
                Salt = salt,
                Role = _context.Roles.SingleOrDefault(r => r.RoleId == 1)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();


            return Ok();
        }
    }
}
