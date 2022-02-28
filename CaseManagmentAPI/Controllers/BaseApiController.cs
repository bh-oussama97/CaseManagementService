
using Microsoft.Extensions.DependencyInjection;
using Application.Core;

using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace CaseManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());


    }
}       
