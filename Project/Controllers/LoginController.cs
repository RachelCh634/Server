﻿using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MODELS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public struct Login
{
    public string Mail { get; set; }
    public string Id { get; set; }
    public Login(string mail, string id)
    {
        Mail = mail;
        Id = id;
    }
}

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _user;

        public LoginController(IConfiguration config, IUserService user)
        {
            _config = config;
            _user = user;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Login loginRequest)
        {
            var userFind = await _user.GetUserById(loginRequest.Id);

            if (userFind != null)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFind.Id),
                    new Claim(ClaimTypes.Role, userFind.Role) 
                };

                var tokenDescriptor = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddMonths(1),
                    signingCredentials: credentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                return Ok(new { token });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}