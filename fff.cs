using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PracticaCST.Data;
using PracticaCST.Data.Entities;
using PracticaCST.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PracticaCST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly SocMediaDb _db;
        private readonly IConfiguration _config;

        public AccountController(SocMediaDb db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDTO payload)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == payload.Username);
            if (existingUser == null || !VerifyPassword(payload.Password, existingUser.HashedPassword))
            {
                return NotFound();
            }

            var jwt = GenerateJSONWebToken(existingUser);
            return new JsonResult(new { jwt });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationDTO payload)
        {
            if (await _db.Users.AnyAsync(u => u.Email == payload.Email))
            {
                return BadRequest("Email address is already in use.");
            }

            var newUser = new User
            {
                Email = payload.Email,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                BirthDate = payload.BirthDate,
                Gender = payload.Gender
            };
            newUser.HashedPassword = HashPassword(payload.Password);

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            return new JsonResult(new { Message = "User created successfully." });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                var base64hashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);
                return base64hashedPasswordBytes == hashedPassword;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("profile_picture_url","..."),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
