using Application.DTO;
using AutoMapper;
using Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Companies
{
    public class GetCompany
    {

        public class GetCompanyByIdQuery : IRequest<CompanyDTO>
        {
            public string AdminId { get; set; }

        }



        public class Handler : IRequestHandler<GetCompanyByIdQuery, CompanyDTO>
        {

            private readonly IMapper _mapper;
            private readonly ApplicationDbContext _context;
   

            public Handler(IMapper mapper, ApplicationDbContext context)
            {
                _mapper = mapper;
                _context = context;
              
            }

            public  Task<CompanyDTO> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
            {
                try

                {
                    Company cabinet = _context.Companies.Where(r => r.AdminId == request.AdminId).FirstOrDefault();


                    var result = cabinet != null ? _mapper.Map<Company, CompanyDTO>(cabinet) : null;

                    return Task.FromResult(result);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

    }
}

