﻿using Core.Models;
using ECommerceGP.Bl.Dtos.UserDtos;
using ECommerceGP.Bl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        public AccountsController(UserManager<User> _userManager,IConfiguration _configuration)
        {
            this.userManager = _userManager;
            this.configuration = _configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> register(UserRegisterDTO input)
        {
            User NewUser = new User
            {
                FullName = input.FullName,
                UserName = input.UserName,
                Email = input.Email,
                Address = input.Address,
                PhoneNumber = input.PhoneNumber
            };

            var creationResult = await userManager.CreateAsync(NewUser, input.password);

            if (!creationResult.Succeeded)
            {
                return BadRequest(creationResult.Errors);

            }
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier,NewUser.Id.ToString()),
               new Claim(ClaimTypes.Role,"Client")
            };

            var claimsResult = await userManager.AddClaimsAsync(NewUser, claims);
            if (!claimsResult.Succeeded)
            {
                return BadRequest(claimsResult.Errors);
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]

        public async Task<ActionResult<TokenDTO>> Login(LoginCredentialsDTO loginInput)
        {
            var user = await userManager.FindByNameAsync(loginInput.username);

            if (user == null)
            {
                return BadRequest(new { message = "user not found" });

            }

            var Islocked = await userManager.IsLockedOutAsync(user);
            if (Islocked)
            {
                return BadRequest(new { message = "You  are Locked Out" });
            }

            if (!await userManager.CheckPasswordAsync(user, loginInput.password))
            {
                await userManager.AccessFailedAsync(user);
                return Unauthorized();
            }

            // get the claims of the user where the input value (user) contain all the values of the logged in user
            var userclaims = await userManager.GetClaimsAsync(user);

            var KeyString = configuration.GetValue<string>("SecretKey");
            var KeyInBytes = Encoding.ASCII.GetBytes(KeyString);
            var Key = new SymmetricSecurityKey(KeyInBytes);

            var signingCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);



            var jwt = new JwtSecurityToken(
                claims: userclaims,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddMinutes(30),
                notBefore: DateTime.Now

                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);
            return Ok(new TokenDTO
            {
                Token = tokenString
            });
        }
        
        [HttpGet]
        [Authorize]
        [Route("CurrentUser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var CurrentUser = await userManager.GetUserAsync(User);
            return Ok(
                new
                {
                    Id = CurrentUser.Id,
                    UserName = CurrentUser.UserName
                }
                );
        }
    }
}
