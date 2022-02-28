
using Application.DTO;
using Application.Users;
using AutoMapper;
using CaseManagmentAPI.Services;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CaseManagmentAPI.Controllers
{

   
    public class UserController : BaseApiController

    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> siginInmanager;
        private readonly TokenService tokenservice;
        private readonly IMapper mapper;

        public UserController(UserManager<AppUser> userManager,
                SignInManager<AppUser> siginInmanager,
                TokenService tokenservice, IMapper mapper)
        {
            this.userManager = userManager;
            this.siginInmanager = siginInmanager;
            this.tokenservice = tokenservice;
            this.mapper = mapper;
        }


        [HttpGet("currentUser")]

        public async Task<ActionResult<UserDTO>> getCurrentUser()
        {


                //AppUser currentuser = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            return CreateUserObject(user);

        
        }
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO userForRegistration)
        {


            var user = mapper.Map<AppUser>(userForRegistration);


            user.Email = userForRegistration.Email;
            user.UserName = userForRegistration.Username;
            user.CompanyDataBaseName = "";
            user.NormalizedUserName = userForRegistration.Email;
            user.Id = Guid.NewGuid().ToString();


            if (userManager.Users.Any(x => x.Email == userForRegistration.Email))
            {
                return BadRequest("Email Taken");

            }

            if (userManager.Users.Any(x => x.UserName == userForRegistration.Username))
            {
                return BadRequest("Username Taken");
            }





            var result = await userManager.CreateAsync(user, userForRegistration.Password);

            if (!result.Succeeded)

            {

                return BadRequest("Problem registering user");
               

            }

            else
            {

                await userManager.AddToRoleAsync(user, userForRegistration.role.NormalizedName);

                return CreateUserObject(user);
            }

           
        }



        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> Login(LoginDTO logindto)
        {

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == logindto.Email);

            if (user == null) return Unauthorized();

            var result = await siginInmanager.CheckPasswordSignInAsync(user, logindto.Password, false);

            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }

            return Unauthorized();

        }


        [AllowAnonymous]
        [HttpPost("SendPasswordResetCode")]
      
        public async Task<IActionResult> SendPasswordResetCode(string email)
        {
           
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email should not be null or empty");
            }

         
                var res=   await Mediator.Send(new CreateResetPassword.ResetPasswordCommand { email = email });

                return Ok(res);

            
                 }



        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Email & New Password should not be null or empty");
            }


            var res = await Mediator.Send(new CreateNewPassword.Command { Code = otp, Email = email, NewPassword = newPassword });

            
            return Ok(res);
        }


        [HttpPut("ChangePassword")]
        [AllowAnonymous]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePassword)
        {

            var user = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

          
            var res = await userManager.ChangePasswordAsync(user,
                changePassword.CurrentPassword, changePassword.NewPassword);

            if (!res.Succeeded)
            {
                return BadRequest("Verify your current password !");
            }

            return Ok("Password changed !!");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> LogOff()
        {
            await siginInmanager.SignOutAsync();

            return Ok("loagged out .");
           

        }


        private UserDTO CreateUserObject(AppUser user)
        {
            return new UserDTO
            {
                Email = user.Email,
                Username = user.UserName,
                Token = tokenservice.createToken(user)
            };
        }
    }
}
