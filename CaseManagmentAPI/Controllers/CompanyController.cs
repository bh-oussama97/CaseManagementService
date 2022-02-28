using Application.Companies;
using Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseManagmentAPI.Controllers
{
   
    public class CompanyController : BaseApiController
    {

        [HttpPost("CreateCompany")]
 

        public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO companymodel)
        {
            if (companymodel == null || ! ModelState.IsValid)
                return BadRequest();


            var result = await Mediator.Send(new CreateCompany.CompanyCommand { companyModel = companymodel });


            if (result == null)


            {
                return BadRequest("error");
            }


            return Ok(result);
        }



        [HttpGet("[action]")]
        [Produces(typeof(CompanyDTO))]

        public async Task<IActionResult> getCompanyByAdmin([FromQuery] string adminId)
        {
            if (adminId == null || !ModelState.IsValid)
                return BadRequest();

            var company = await Mediator.Send(new GetCompany.GetCompanyByIdQuery { AdminId = adminId });

            return Ok(company);
        }

    }
}
