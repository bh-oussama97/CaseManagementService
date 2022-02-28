using Application.DTO;
using Application.Users;
using AutoMapper;
using AutoMapper.Configuration;
using Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Roles
{
    public class CreateRole
    {

        public class CreateRoleCommand : IRequest<CommandResult>
        {
            public RoleDTO roleModel { get; set; }

        }


        public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CommandResult>
        {

            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;
            private readonly RoleManager<Role> _roleManager;

            public CreateRoleCommandHandler(IMapper mapper, IConfiguration configuration, RoleManager<Role> roleManager)
            {
                _mapper = mapper;
                _configuration = configuration;
                _roleManager = roleManager;


            }
            private bool roleexistes(string name)
            {
                return _roleManager.Roles.Any(x => x.Name == name);

            }
            public async Task<CommandResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                CommandResult result = new CommandResult();

                try
                {

                    if (roleexistes(request.roleModel.Name))
                    {

                        result.Success = false;
                        result.Message = "role déjà existe";

                    }
                    else
                    {
                        var role = _mapper.Map<Role>(request.roleModel);
                        role.Name = request.roleModel.Name;
                        role.Id = Guid.NewGuid().ToString();
                        var res = await _roleManager.CreateAsync(role);


                        if (!res.Succeeded)
                        {
                            result.Success = false;
                            result.Message = "Ajout du role est échoué";
                        }


                        else
                        {
                            result.Success = true;
                            result.Message = "role est ajouté avec succès!";
                        }

                    }
                    return result;
                }
                catch (Exception e)
                {
                    result.Success = false;
                    return result;
                }
            }
        }
    }
}
