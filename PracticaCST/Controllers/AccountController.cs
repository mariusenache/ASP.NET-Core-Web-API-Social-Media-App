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
        private readonly SocMediaDb _db;           //readonly = assignable doar in ctor
        private IConfiguration _config;             //obiect care ma ajuta sa citesc din appsettings.json

        public AccountController(SocMediaDb db, IConfiguration config)
        {
            _db = db;                               //acum am BD
            _config = config;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDTO payload)  //din DTO: am acces la username si pass cu care a venit cnv; de verificat daca le am in BD
        {
            string base64hashedPasswordBytes;
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(payload.Password);   
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                base64hashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);
            }

            var existingUser = _db.Users
                .Where(u => u.Email == payload.Username
                    && u.HashedPassword == base64hashedPasswordBytes)
                .SingleOrDefault();
            if(existingUser is null)
            {
                return NotFound();
            }
            else
            {
                var jwt = GenerateJSONWebToken(existingUser);
                return new JsonResult(new { jwt });
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
 