using AA_Aggregation_API_AT.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AA_Aggregation_API_AT.Authentication.Controllers
{
    [ApiController]                
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _userName;
        private readonly string _userPassword;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _userName = _config["Jwt:User"]!;
            _userPassword = _config["Jwt:Password"]!;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDto login)
        {
            if (login.UserName != _userName || login.Password != _userPassword)
                return Unauthorized();


            var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, login.UserName),
                            new Claim("role", "User")
                        };

            var key = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                               issuer: _config["Jwt:Issuer"],
                               audience: _config["Jwt:Audience"],
                               claims: claims,
                               expires: DateTime.UtcNow.AddHours(3),
                               signingCredentials: creds
                               );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
