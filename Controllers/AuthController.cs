using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tickets.API.Data;
using Tickets.API.Dtos;
using Tickets.API.Models;

namespace Tickets.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForCreateDto userForCreateDto)
        {
            // Check if user exists
            if (!string.IsNullOrEmpty(userForCreateDto.Username))
                userForCreateDto.Username = userForCreateDto.Username.ToLower();
            
            var exists = await _repo.UserExists(userForCreateDto.Username);

            if (exists)
                ModelState.AddModelError("Username", "Username already exists");
            
            // Validate request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create user
            var userToCreate = new User
            {
                Username = userForCreateDto.Username,
                Email = userForCreateDto.Email,
                Address = userForCreateDto.Address
            };

            var user = await _repo.Register(userToCreate, userForCreateDto.Password);

            // Generate token
            var token = GenerateToken(user);

            return Ok(new { token, user });

            // return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            // Handle login
            var user = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (user == null)
                return Unauthorized();

            // Generate token
            var token = GenerateToken(user);
            

            return Ok(new { token, user });
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}