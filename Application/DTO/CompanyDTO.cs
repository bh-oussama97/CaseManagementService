using Application.Interfaces;
using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CompanyDTO : IHaveCustomMapping
    {
        public string Name { get; set; }

        public string DataBaseName { get; set; }

        public string AdminId { get; set; }


        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Company, CompanyDTO>();
        }

    }
}
