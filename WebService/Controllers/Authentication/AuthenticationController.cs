using DatabaseService;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using WebService.DTOs;
using WebService.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using DatabaseService.Modules;

namespace WebService.Controllers.Authentication
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAppUserService _service;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAppUserService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("users")]
        public ActionResult CreateUser([FromBody] SignupUserDto dto)
            //changed to FromForm just because i cant be bothered to look up what to send in postman
        {
            if (_service.GetAppUser(dto.Username) != null)
            {
                return BadRequest();
            }

            int.TryParse(
                _configuration.GetSection("Auth:PwdSize").Value,
                out var size);

            if (size == 0)
            {
                throw new ArgumentException();
            }

            var salt = PasswordService.GenerateSalt(size);

            var pwd = PasswordService.HashPassword(dto.Password, salt, size);

            _service.CreateUser(dto.Username, pwd, salt);

            return CreatedAtRoute(null, dto.Username);
        }


        [HttpPost("tokens")]
        public ActionResult Login([FromBody] SignupUserDto dto)
        //changed to FromForm just because i cant be bothered to look up what to send in postman
        {
            var user = _service.GetAppUser(dto.Username);

            if (user == null)
            {
                return BadRequest();
            }

            if(IsInvalidPassword(dto, user))
            {
                return BadRequest();
            }

            var userToken = GenerateToken(user);
            return Ok(new { user.Username, userToken });

        }

        private bool IsInvalidPassword(SignupUserDto dto, AppUser user)
        {
            int.TryParse(
               _configuration.GetSection("Auth:PwdSize").Value,
               out var size);
            var pwd = PasswordService.HashPassword(dto.Password, user.Salt, size);

            if (user.Password != pwd)
            {
                return true;
            }
            return false;
        }
        private string GenerateToken(AppUser user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Auth:Key"]);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()), //need to see what are claims and how they work
                    //as i understand it, we can use the claim to get the user/name/id from the other controllers
                }),
                //Expires = DateTime.Now.AddMinutes(3),
                Expires = DateTime.Now.AddDays(1), //when testing functions
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescription);
            var token = tokenHandler.WriteToken(securityToken);
            return token;

        }

    }
}
