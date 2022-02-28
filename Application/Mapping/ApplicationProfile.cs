using Application.DTO;
using AutoMapper;
using Entities;

namespace CaseManagmentAPI.Application.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {

            CreateMap<AppUser, RegisterDTO>().ReverseMap();
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<AppUser, UserDTO>().ReverseMap();
            //CreateMap<Collaborateur, CollaborateurDto>().ReverseMap();
            //CreateMap<Role, RoleDTO>().ReverseMap();
        }
    }
}
