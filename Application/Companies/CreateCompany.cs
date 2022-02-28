using Application.DTO;
using AutoMapper;
using Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Companies
{
    public class CreateCompany
    {

        public class CompanyCommand : IRequest<CommandResult>
        {
            public CompanyDTO companyModel { get; set; }
        }


        public class Handler : IRequestHandler<CompanyCommand, CommandResult>
        {
            private readonly IMapper _mapper;
            private readonly ApplicationDbContext datacontext;
            private readonly UserManager<AppUser> userManager;
            private readonly IConfiguration _configuration;

            public Handler(IMapper mapper, ApplicationDbContext datacontext, UserManager<AppUser> userManager, IConfiguration configuration)
            {
                this._mapper = mapper;
                this.datacontext = datacontext;
                this.userManager = userManager;
                this._configuration = configuration;
            }

            public async Task<CommandResult> Handle(CompanyCommand request, CancellationToken cancellationToken)
            {


                CommandResult resultOp = new CommandResult();

                var appName = _configuration.GetSection("SetConnectionString:AppName").Value;
                var dataBaseName = appName + "_" + request.companyModel.Name;//+ request.cabinetModel.headOffice.Substring(0, 5);
                var connectionString =
                    _configuration.GetSection("SetConnectionString:DataSource").Value +
                    _configuration.GetSection("SetConnectionString:Credentials").Value +
                    "Initial Catalog=" + dataBaseName;

                var user = await userManager.FindByIdAsync(request.companyModel.AdminId);


                if (user == null)

                {
                    resultOp.Success = false;
                    resultOp.Message = "user admin n'existe pas";
                }

                user.CompanyDataBaseName = dataBaseName;

                var company = _mapper.Map<Company>(request.companyModel);

                if (VerifCompany(company.Name))
                {

                    resultOp.Success = false;
                    resultOp.Message = "Company existe";

                }
                else
                {
                    try
                    {

                        company.Id = Guid.NewGuid();
                        company.DataBaseName = dataBaseName;
                        datacontext.Companies.Add(company);



                        await datacontext.Database.ExecuteSqlInterpolatedAsync
                            ($"EXEC [dbo].[SP_CreateDataBase] @dataBaseName={dataBaseName}");

                        await userManager.UpdateAsync(user);

                      
                        await datacontext.SaveChangesAsync(cancellationToken);

                        resultOp.Success = true;
                        resultOp.Message = "company created Successfully";



                    }
                    catch (Exception ex)
                    {
                        resultOp.Success = false;
                        resultOp.Message = ex.Message;


                    }
                }


                return resultOp;
            }

            private bool VerifCompany(string name)
            {
                return datacontext.Companies.Any(x => x.Name == name);

            }
        }
    }

}
