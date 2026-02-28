using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Models;
using DomainServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebService.DTOs;
using WebService.Services;

namespace WebService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("users")]
    public ActionResult CreateUser([FromBody] SignupUserDto dto)
    {
        if (!IsValidUserCredential(dto))
        {
            return BadRequest();
        }

        if (_userService.UserExists(dto.Username))
        {
            return BadRequest("Username already exists");
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

        _userService.CreateUser(dto.Username, pwd, salt);

        return CreatedAtRoute(null, dto.Username);
    }


    [HttpPost("tokens")]
    public ActionResult Login([FromBody] SignupUserDto dto)
    {
        if (!IsValidUserCredential(dto)) { return BadRequest(); }

        var user = _userService.GetAppUser(dto.Username);
        if (user is null || IsInvalidPassword(dto, user))
        {
            return BadRequest();
        }

        var userToken = GenerateToken(user);
        var result = new AuthenticatedUser
        {
            Username = user.Username,
            Token = userToken
        };
        
        return Ok(result);
    }

    private bool IsInvalidPassword(SignupUserDto dto, AppUser user)
    {
        int.TryParse(
            _configuration.GetSection("Auth:PwdSize").Value,
            out var size);
        var pwd = PasswordService.HashPassword(dto.Password, user.Salt, size);

        return user.Password != pwd;
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

    private static bool IsValidUserCredential(SignupUserDto dto)
    {
        var isUsernameOrPasswordEmpty = string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password);
        if (isUsernameOrPasswordEmpty)
        {
            return false;
        }

        var isUsernameOrPasswordToShort = dto.Username.Length < 2 || dto.Password.Length < 6;
        if (isUsernameOrPasswordToShort)
        {
            return false;
        }

        const string regExUsernameInvalidValue = @"[^a-zA-Z\d]";
        const string regExPasswordInvalidValue = @"[^a-zA-Z\d]";
        var regExMatchInvalidUser = Regex.Match(dto.Username, regExUsernameInvalidValue, RegexOptions.IgnoreCase);
        var regExMatchInvalidPassword = Regex.Match(dto.Password, regExPasswordInvalidValue, RegexOptions.IgnoreCase);

        if (!regExMatchInvalidUser.Success &&
            !regExMatchInvalidPassword.Success)
        {
            return true;
        }
        
        Console.WriteLine("Am entering successfully :D ");
        return false;
    }
}