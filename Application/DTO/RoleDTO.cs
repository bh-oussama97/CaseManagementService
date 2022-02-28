using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public  class RoleDTO : IHaveCustomMapping

    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<RoleDTO, IdentityRole>()
                .ForMember(u => u.Name, opt => opt.MapFrom(x => x.Name));
        }




    }
}
