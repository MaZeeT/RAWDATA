using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application;
using Application.UseCases.Users.CreateUser;
using Application.UseCases.Users.LoginUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Web.DTOs;

namespace Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ICreateUser _createUser;
    private readonly ILoginUser _loginUser;

    public AuthenticationController(IConfiguration configuration, ICreateUser createUser, ILoginUser loginUser)
    {
        _configuration = configuration;
        _createUser = createUser;
        _loginUser = loginUser;
    }

    [HttpPost("users")]
    public ActionResult CreateUser([FromBody] SignupUserDto dto)
    {
        AuthSettings authSettings;
        try
        {
            authSettings = ReadAuthSettings();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }

        var createUserCommand = new CreateUserCommand
        {
            Username = dto.Username,
            Password = dto.Password
        };

        var result = _createUser.Execute(createUserCommand, authSettings);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtRoute(null, result.Value.Username);
    }


    [HttpPost("tokens")]
    public ActionResult Login([FromBody] SignupUserDto dto)
    {
        
        var command = new LoginUserCommand
        {
            Username = dto.Username,
            Password = dto.Password
        };

        var result = _loginUser.Execute(command);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(result.Error);

        var userToken = GenerateToken(result.Value);

        return Ok(new AuthenticatedUser
        {
            Username = result.Value.UserName,
            Token = userToken
        });
    }

    private AuthSettings ReadAuthSettings()
    {
        if (!int.TryParse(
                _configuration.GetSection("Auth:PwdSize").Value,
                out var pwdSize))
        {
            throw new ConfigurationErrorsException("Could not parse Auth:PwdSize to an int");
        }

        if (pwdSize == 0)
        {
            throw new ConfigurationErrorsException("Auth:PWD size must be greater than zero");
        }

        var authSettings = new AuthSettings
        {
            PasswordSize = pwdSize,
        };
        return authSettings;
    }

    private string GenerateToken(LoginUserResult user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Auth:Key"]);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()), //need to see what are claims and how they work
                //as I understand it, we can use the claim to get the user/name/id from the other controllers
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