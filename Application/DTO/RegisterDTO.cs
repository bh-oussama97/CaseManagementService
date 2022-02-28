using Application.Interfaces;
using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RegisterDTO : IHaveCustomMapping
    {

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string confirmPassword { get; set; }

   
        [Required(ErrorMessage = "Role is required")]
        public RoleDTO role { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<RegisterDTO, AppUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Username))

                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email));
        }


    }
}
